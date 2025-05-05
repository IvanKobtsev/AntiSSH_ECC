using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AntiSSH.Auth.ECC.Filters;

public class OpenApiSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        foreach (var property in context.Type.GetProperties())
        {
            if (property.GetCustomAttribute<PhoneAttribute>() != null)
            {
                var propertyName = char.ToLowerInvariant(property.Name[0]) + property.Name[1..];
                if (schema.Properties.TryGetValue(propertyName, out var schemaProperty))
                    schemaProperty.Example = new OpenApiString("89000000000");
            }

            if (property.GetCustomAttribute<PasswordPropertyTextAttribute>() != null)
            {
                var propertyName = char.ToLowerInvariant(property.Name[0]) + property.Name[1..];
                if (schema.Properties.TryGetValue(propertyName, out var schemaProperty))
                    schemaProperty.Example = new OpenApiString("Passw@rd1");
            }
        }
    }
}