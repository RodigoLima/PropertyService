using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyService.Application.Services;

namespace PropertyService.Api.Controllers;

[ApiController]
[Authorize]
public abstract class BaseController : ControllerBase
{
    protected readonly IUserContextService UserContext;

    protected BaseController(IUserContextService userContext)
    {
        UserContext = userContext;
    }

    protected Guid GetProdutorId() => UserContext.GetProdutorId();
}
