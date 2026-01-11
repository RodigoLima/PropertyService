using PropertyService.Domain.Entities;

namespace PropertyService.Application.Interfaces;

public interface IPropriedadeRepository
{
    Task<Propriedade?> ObterPorIdAsync(Guid id);
    Task<Propriedade?> ObterPorIdEProdutorIdAsync(Guid id, Guid produtorId);
    Task<IEnumerable<Propriedade>> ObterTodasAsync();
    Task<IEnumerable<Propriedade>> ObterPorProdutorIdAsync(Guid produtorId);
    Task<Propriedade> CriarAsync(Propriedade propriedade);
    Task<Propriedade> AtualizarAsync(Propriedade propriedade);
    Task<bool> ExcluirAsync(Guid id);
}
