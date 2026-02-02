using AgroSolutions.Contracts;
using MassTransit;
using PropertyService.Application.Interfaces;
using PropertyService.Domain.Entities;

namespace PropertyService.Application.Services;

public class TalhaoService
{
    private readonly ITalhaoRepository _talhaoRepository;
    private readonly IPropriedadeRepository _propriedadeRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public TalhaoService(ITalhaoRepository talhaoRepository, IPropriedadeRepository propriedadeRepository, IPublishEndpoint publishEndpoint)
    {
        _talhaoRepository = talhaoRepository;
        _propriedadeRepository = propriedadeRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Talhao?> ObterPorIdEProdutorIdAsync(Guid id, Guid produtorId)
    {
        var talhao = await _talhaoRepository.ObterPorIdAsync(id);
        if (talhao == null)
            return null;

        var propriedade = await _propriedadeRepository.ObterPorIdEProdutorIdAsync(talhao.PropriedadeId, produtorId);
        return propriedade == null ? null : talhao;
    }

    public async Task<IEnumerable<Talhao>> ObterPorPropriedadeIdEProdutorIdAsync(Guid propriedadeId, Guid produtorId)
    {
        var propriedade = await _propriedadeRepository.ObterPorIdEProdutorIdAsync(propriedadeId, produtorId);
        if (propriedade == null)
            return Enumerable.Empty<Talhao>();

        return await _talhaoRepository.ObterPorPropriedadeIdAsync(propriedadeId);
    }

    public async Task<Talhao?> CriarAsync(Guid propriedadeId, Guid produtorId, string nome, string cultura, string? descricao = null, decimal? areaHectares = null)
    {
        var propriedade = await _propriedadeRepository.ObterPorIdEProdutorIdAsync(propriedadeId, produtorId);
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

        var created = await _talhaoRepository.CriarAsync(talhao);
        await _publishEndpoint.Publish(new TalhaoDataMessage(created.Id, created.Nome, created.PropriedadeId));
        return created;
    }

    public async Task<Talhao?> AtualizarAsync(Guid id, Guid produtorId, string nome, string cultura, string? descricao = null, decimal? areaHectares = null)
    {
        var talhao = await ObterPorIdEProdutorIdAsync(id, produtorId);
        if (talhao == null)
            return null;

        talhao.Nome = nome;
        talhao.Cultura = cultura;
        talhao.Descricao = descricao;
        talhao.AreaHectares = areaHectares;

        var updated = await _talhaoRepository.AtualizarAsync(talhao);
        await _publishEndpoint.Publish(new TalhaoDataMessage(updated.Id, updated.Nome, updated.PropriedadeId));
        return updated;
    }

    public async Task<bool> ExcluirAsync(Guid id, Guid produtorId)
    {
        var talhao = await ObterPorIdEProdutorIdAsync(id, produtorId);
        return talhao != null && await _talhaoRepository.ExcluirAsync(id);
    }
}
