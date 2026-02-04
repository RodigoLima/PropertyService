using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace PropertyService.Api.Configuration;

// Handler que sempre autentica (apenas para desenvolvimento).
internal sealed class DevelopmentBypassHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public DevelopmentBypassHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim("sub", DevelopmentBypassConstants.ProdutorId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, DevelopmentBypassConstants.ProdutorId.ToString()),
            new Claim("produtorId", DevelopmentBypassConstants.ProdutorId.ToString())
        };

        var identity = new ClaimsIdentity(claims, DevelopmentBypassConstants.SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

