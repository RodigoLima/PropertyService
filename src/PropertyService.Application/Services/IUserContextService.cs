namespace PropertyService.Application.Services;

public interface IUserContextService
{
    Guid GetProdutorId();
    bool IsAuthenticated { get; }
}
