using ManagementService.Application.Interfaces;
using ManagementService.Domain.Entities;

namespace ManagementService.Application.Services;

public class TalhaoService
{
    private readonly ITalhaoRepository _talhaoRepository;
    private readonly IPropriedadeRepository _propriedadeRepository;

    public TalhaoService(ITalhaoRepository talhaoRepository, IPropriedadeRepository propriedadeRepository)
    {
        _talhaoRepository = talhaoRepository;
        _propriedadeRepository = propriedadeRepository;
    }

    public async Task<Talhao?> ObterPorIdAsync(Guid id)
    {
        return await _talhaoRepository.ObterPorIdAsync(id);
    }

    public async Task<IEnumerable<Talhao>> ObterPorPropriedadeIdAsync(Guid propriedadeId)
    {
        return await _talhaoRepository.ObterPorPropriedadeIdAsync(propriedadeId);
    }

    public async Task<Talhao?> CriarAsync(Guid propriedadeId, string nome, string cultura, string? descricao = null, decimal? areaHectares = null)
    {
        // Verificar se a propriedade existe
        var propriedade = await _propriedadeRepository.ObterPorIdAsync(propriedadeId);
        if (propriedade == null)
            return null;

        var talhao = new Talhao
        {
            PropriedadeId = propriedadeId,
            Nome = nome,
            Cultura = cultura,
            Descricao = descricao,
            AreaHectares = areaHectares
        };

        return await _talhaoRepository.CriarAsync(talhao);
    }

    public async Task<Talhao?> AtualizarAsync(Guid id, string nome, string cultura, string? descricao = null, decimal? areaHectares = null)
    {
        var talhao = await _talhaoRepository.ObterPorIdAsync(id);
        if (talhao == null)
            return null;

        talhao.Nome = nome;
        talhao.Cultura = cultura;
        talhao.Descricao = descricao;
        talhao.AreaHectares = areaHectares;

        return await _talhaoRepository.AtualizarAsync(talhao);
    }

    public async Task<bool> ExcluirAsync(Guid id)
    {
        return await _talhaoRepository.ExcluirAsync(id);
    }
}
