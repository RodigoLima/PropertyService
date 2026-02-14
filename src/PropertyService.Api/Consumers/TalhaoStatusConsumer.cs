using AgroSolutions.Contracts;
using MassTransit;
using PropertyService.Application.Interfaces;
using PropertyService.Domain.Entities;

namespace PropertyService.Api.Consumers;

public class TalhaoStatusConsumer(ITalhaoRepository _talhaoRepository, ILogger<TalhaoStatusConsumer> _logger)
    : IConsumer<TalhaoStatusUpdateMessage>
{
    public async Task Consume(ConsumeContext<TalhaoStatusUpdateMessage> context)
    {
        var msg = context.Message;
        if (msg.Status < 1 || msg.Status > 3)
        {
            _logger.LogWarning("Status inválido {Status} ignorado para talhão {TalhaoId}", msg.Status, msg.TalhaoId);
            return;
        }
        var status = (StatusTalhao)msg.Status;
        var ok = await _talhaoRepository.AtualizarStatusAsync(msg.TalhaoId, status);
        if (ok)
            _logger.LogInformation("Talhão {TalhaoId} atualizado para status {Status}", msg.TalhaoId, status);
        else
            _logger.LogWarning("Talhão {TalhaoId} não encontrado para atualização de status", msg.TalhaoId);
    }
}
