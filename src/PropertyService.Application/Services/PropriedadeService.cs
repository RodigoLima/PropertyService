using AgroSolutions.Contracts;
using MassTransit;
using PropertyService.Application.Interfaces;
using PropertyService.Domain.Entities;

namespace PropertyService.Application.Services;

public class PropriedadeService
{
    private readonly IPropriedadeRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public PropriedadeService(IPropriedadeRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Propriedade?> ObterPorIdAsync(Guid id, Guid produtorId)
        => await _repository.ObterPorIdEProdutorIdAsync(id, produtorId);

    public async Task<IEnumerable<Propriedade>> ObterPorProdutorIdAsync(Guid produtorId)
        => await _repository.ObterPorProdutorIdAsync(produtorId);

    public async Task<Propriedade> CriarAsync(Guid produtorId, string nome, string? descricao = null)
    {
        var propriedade = new Propriedade
        {
            ProdutorId = produtorId,
            Nome = nome,
            Descricao = descricao
        };

        var created = await _repository.CriarAsync(propriedade);
        await _publishEndpoint.Publish(new PropriedadeDataMessage(created.Id, created.Nome, created.ProdutorId));
        return created;
    }

    public async Task<Propriedade?> AtualizarAsync(Guid id, Guid produtorId, string nome, string? descricao = null)
    {
        var propriedade = await _repository.ObterPorIdEProdutorIdAsync(id, produtorId);
        if (propriedade == null)
            return null;

        propriedade.Nome = nome;
        propriedade.Descricao = descricao;

        var updated = await _repository.AtualizarAsync(propriedade);
        await _publishEndpoint.Publish(new PropriedadeDataMessage(updated.Id, updated.Nome, updated.ProdutorId));
        return updated;
    }

    public async Task<bool> ExcluirAsync(Guid id, Guid produtorId)
    {
        var propriedade = await _repository.ObterPorIdEProdutorIdAsync(id, produtorId);
        return propriedade != null && await _repository.ExcluirAsync(id);
    }
}
