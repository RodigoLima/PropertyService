using PropertyService.Api.Configuration;
using PropertyService.Application.Services;
using System.Security.Claims;

namespace PropertyService.Api.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserContextService> _logger;

    public UserContextService(IHttpContextAccessor httpContextAccessor, ILogger<UserContextService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public Guid GetProdutorId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
            throw new UnauthorizedAccessException("Usuário não autenticado.");

        if (string.Equals(user.Identity?.AuthenticationType, DevelopmentBypassConstants.SchemeName, StringComparison.Ordinal))
        {
            _logger.LogInformation("Bypass de JWT ativo (Development:DisableJwtValidation). ProdutorId: {ProdutorId}", DevelopmentBypassConstants.ProdutorId);
            return DevelopmentBypassConstants.ProdutorId;
        }

        var claim = user.FindFirst("sub")
                 ?? user.FindFirst(ClaimTypes.NameIdentifier)
                 ?? user.FindFirst("userId")
                 ?? user.FindFirst("produtorId");

        if (claim == null || !Guid.TryParse(claim.Value, out var produtorId))
            throw new UnauthorizedAccessException("UserId não encontrado no token.");

        return produtorId;
    }
}
