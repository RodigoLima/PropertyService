using PropertyService.Domain.Entities;

namespace PropertyService.Application.Interfaces;

public interface ITalhaoRepository
{
    Task<Talhao?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Talhao>> ObterPorPropriedadeIdAsync(Guid propriedadeId);
    Task<Talhao> CriarAsync(Talhao talhao);
    Task<Talhao> AtualizarAsync(Talhao talhao);
    Task<bool> ExcluirAsync(Guid id);
}
