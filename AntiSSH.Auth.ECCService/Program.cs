using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using AntiSSH.Auth.ECC.Data;
using AntiSSH.Auth.ECC.Filters;
using AntiSSH.Auth.ECC.Models;
using AntiSSH.Auth.ECC.Services;
using AntiSSH.Auth.ECC.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});

builder
    .Services.AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        builder.Configuration["OpenApiInfo:Version"],
        new OpenApiInfo
        {
            Title = builder.Configuration["OpenApiInfo:Title"],
            Version = builder.Configuration["OpenApiInfo:Version"],
        }
    );
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Please enter your token. Swagger will automatically append the 'Bearer' at the beginning",
        }
    );

    options.OperationFilter<SecurityRequirementsOperationFilter>();
    options.SchemaFilter<OpenApiSchemaFilter>();
});

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddSingleton<CustomEcdsaService>();

builder
    .Services.AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .AddNewtonsoftJson(x =>
    {
        x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        x.SerializerSettings.Converters.Add(new StringEnumConverter());
    });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder
    .Services.AddAuthentication("CustomScheme")
    .AddScheme<AuthenticationSchemeOptions, CustomAuthHandler>("CustomScheme", null);
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("AllowAll");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();

    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying migrations: {ex.Message}");
    }

    var foundEccPrivateKey = dbContext.EncryptedKeys.FirstOrDefault(x => x.Name == "ECCPrivateKey");

    if (foundEccPrivateKey == null)
    {
        try
        {
            var privateKey = File.ReadAllBytes("./SecretVault/my-private-key.txt");
            var passphrase = File.ReadAllText("./SecretVault/passphrase.txt");
            var encryptedPrivateKey = PrivateKeyService.EncryptPrivateKey(
                privateKey,
                passphrase,
                out var salt,
                out var iv
            );

            dbContext.EncryptedKeys.Add(
                new EncryptedKey
                {
                    Id = Guid.NewGuid(),
                    PrivateKey = encryptedPrivateKey,
                    Salt = salt,
                    Iv = iv,
                    Name = "ECCPrivateKey",
                }
            );

            dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Couldn't encrypt the private key: {ex.Message}");
            throw;
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
