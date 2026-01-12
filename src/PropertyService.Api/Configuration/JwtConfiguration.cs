using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace PropertyService.Api.Configuration;

public static class JwtConfiguration
{
    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>() 
            ?? throw new InvalidOperationException("Configuração JWT não encontrada.");

        var isDevelopmentBypass = environment.IsDevelopment() 
            && configuration.GetValue<bool>("Development:DisableJwtValidation", false);

        services.AddAuthentication(options =>
        {
            if (isDevelopmentBypass)
            {
                options.DefaultAuthenticateScheme = "DevelopmentBypass";
                options.DefaultChallengeScheme = "DevelopmentBypass";
            }
            else
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
        })
        .AddScheme<AuthenticationSchemeOptions, DevelopmentBypassHandler>(
            "DevelopmentBypass",
            _ => { })
        .AddJwtBearer(options =>
        {
            if (string.IsNullOrWhiteSpace(jwtSettings.Key))
                throw new InvalidOperationException("JWT Key não configurada.");

            var signingKey = Convert.FromBase64String(jwtSettings.Key);
            var validAudiences = new[] { jwtSettings.Audience }
                .Concat(jwtSettings.ValidAudiences ?? Array.Empty<string>())
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Distinct()
                .ToArray();

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudiences = validAudiences,
                IssuerSigningKey = new SymmetricSecurityKey(signingKey)
            };
        });

        services.AddAuthorization();
    }
}

// Handler que sempre autentica (apenas para desenvolvimento)
internal class DevelopmentBypassHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public DevelopmentBypassHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Cria uma identidade com claims básicas para desenvolvimento
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "dev-user"),
            new Claim("produtorId", "00000000-0000-0000-0000-000000000000")
        };

        var identity = new ClaimsIdentity(claims, "DevelopmentBypass");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "DevelopmentBypass");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
