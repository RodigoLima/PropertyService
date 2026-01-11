using ManagementService.Application.Interfaces;
using ManagementService.Domain.Entities;

namespace ManagementService.Application.Services;

public class PropriedadeService
{
    private readonly IPropriedadeRepository _propriedadeRepository;

    public PropriedadeService(IPropriedadeRepository propriedadeRepository)
    {
        _propriedadeRepository = propriedadeRepository;
    }

    public async Task<Propriedade?> ObterPorIdAsync(Guid id)
    {
        return await _propriedadeRepository.ObterPorIdAsync(id);
    }

    public async Task<IEnumerable<Propriedade>> ObterTodasAsync()
    {
        return await _propriedadeRepository.ObterTodasAsync();
    }

    public async Task<Propriedade> CriarAsync(string nome, string? descricao = null)
    {
        var propriedade = new Propriedade
        {
            Nome = nome,
            Descricao = descricao
        };

        return await _propriedadeRepository.CriarAsync(propriedade);
    }

    public async Task<Propriedade?> AtualizarAsync(Guid id, string nome, string? descricao = null)
    {
        var propriedade = await _propriedadeRepository.ObterPorIdAsync(id);
        if (propriedade == null)
            return null;

        propriedade.Nome = nome;
        propriedade.Descricao = descricao;

        return await _propriedadeRepository.AtualizarAsync(propriedade);
    }

    public async Task<bool> ExcluirAsync(Guid id)
    {
        return await _propriedadeRepository.ExcluirAsync(id);
    }
}
