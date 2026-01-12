using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace PropertyService.Api.Configuration;

public static class JwtConfiguration
{
    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        const string developmentBypassScheme = "DevelopmentBypass";

        var jwtSection = configuration.GetSection("Jwt");
        services.Configure<JwtSettings>(jwtSection);

        var isDevelopmentBypass =
            environment.IsDevelopment()
            && configuration.GetValue("Development:DisableJwtValidation", false);

        var defaultScheme = isDevelopmentBypass
            ? developmentBypassScheme
            : JwtBearerDefaults.AuthenticationScheme;

        var authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = defaultScheme;
            options.DefaultChallengeScheme = defaultScheme;
        });

        if (isDevelopmentBypass)
        {
            authBuilder.AddScheme<AuthenticationSchemeOptions, DevelopmentBypassHandler>(
                developmentBypassScheme,
                _ => { });
        }
        else
        {
            var jwtSettings = jwtSection.Get<JwtSettings>()
                ?? throw new InvalidOperationException("Configuração JWT não encontrada.");

            if (string.IsNullOrWhiteSpace(jwtSettings.Key))
                throw new InvalidOperationException("JWT Key não configurada.");

            byte[] signingKey;
            try
            {
                signingKey = Convert.FromBase64String(jwtSettings.Key);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("JWT Key inválida (esperado Base64).", ex);
            }

            authBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(signingKey)
                };
            });
        }

        services.AddAuthorization();
    }
}
