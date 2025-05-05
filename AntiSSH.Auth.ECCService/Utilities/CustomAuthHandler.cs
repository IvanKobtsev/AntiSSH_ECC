using System.Security.Claims;
using System.Text.Encodings.Web;
using AntiSSH.Auth.ECC.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AntiSSH.Auth.ECC.Utilities;

public class CustomAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    TokenService tokenService
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string authHeader = Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return Task.FromResult(
                AuthenticateResult.Fail("Missing or invalid Authorization header")
            );

        var token = authHeader["Bearer ".Length..].Trim();

        var userId = tokenService.TryValidateToken(token);

        if (userId == null)
            return Task.FromResult(AuthenticateResult.Fail("Invalid token"));

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
