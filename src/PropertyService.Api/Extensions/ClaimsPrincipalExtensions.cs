using System.Security.Claims;

namespace PropertyService.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetProdutorId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("sub") 
                 ?? user.FindFirst("userId") 
                 ?? user.FindFirst("produtorId");
        
        return claim != null && Guid.TryParse(claim.Value, out var produtorId) 
            ? produtorId 
            : null;
    }
}
