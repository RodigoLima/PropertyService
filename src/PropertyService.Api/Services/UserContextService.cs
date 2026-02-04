using PropertyService.Application.Services;
using System.Security.Claims;

namespace PropertyService.Api.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public Guid GetProdutorId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
            throw new UnauthorizedAccessException("Usuário não autenticado.");

        var claim = user.FindFirst("sub")
                 ?? user.FindFirst(ClaimTypes.NameIdentifier)
                 ?? user.FindFirst("userId")
                 ?? user.FindFirst("produtorId");

        if (claim == null || !Guid.TryParse(claim.Value, out var produtorId))
            throw new UnauthorizedAccessException("UserId não encontrado no token.");

        return produtorId;
    }
}
