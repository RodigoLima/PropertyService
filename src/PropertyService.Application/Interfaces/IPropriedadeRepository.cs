using PropertyService.Domain.Entities;

namespace PropertyService.Application.Interfaces;

public interface IPropriedadeRepository
{
    Task<Propriedade?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Propriedade>> ObterTodasAsync();
    Task<Propriedade> CriarAsync(Propriedade propriedade);
    Task<Propriedade> AtualizarAsync(Propriedade propriedade);
    Task<bool> ExcluirAsync(Guid id);
}
