using PropertyService.Api.DTOs;
using PropertyService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace PropertyService.Api.Controllers;

[Route("api/[controller]")]
public class PropriedadesController : BaseController
{
    private readonly PropriedadeService _propriedadeService;
    private readonly ILogger<PropriedadesController> _logger;

    public PropriedadesController(
        PropriedadeService propriedadeService, 
        IUserContextService userContext,
        ILogger<PropriedadesController> logger)
        : base(userContext)
    {
        _propriedadeService = propriedadeService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodas()
    {
        _logger.LogInformation("Obtendo todas as propriedades do produtor {ProdutorId}", GetProdutorId());
        var propriedades = await _propriedadeService.ObterPorProdutorIdAsync(GetProdutorId());
        _logger.LogInformation("Encontradas {Count} propriedades", propriedades.Count());
        return Ok(propriedades);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        _logger.LogInformation("Obtendo propriedade {PropriedadeId}", id);
        var propriedade = await _propriedadeService.ObterPorIdAsync(id, GetProdutorId());
        
        if (propriedade == null)
        {
            _logger.LogWarning("Propriedade {PropriedadeId} não encontrada", id);
            return NotFound();
        }
        
        return Ok(propriedade);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarPropriedadeDto dto)
    {
        var produtorId = GetProdutorId();
        if (produtorId == Guid.Empty)
            return BadRequest("ProdutorId inválido no token.");
        _logger.LogInformation("Criando nova propriedade: {Nome}", dto.Nome);
        var propriedade = await _propriedadeService.CriarAsync(produtorId, dto.Nome, dto.Descricao);
        _logger.LogInformation("Propriedade {PropriedadeId} criada com sucesso", propriedade.Id);
        return CreatedAtAction(nameof(ObterPorId), new { id = propriedade.Id }, propriedade);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarPropriedadeDto dto)
    {
        _logger.LogInformation("Atualizando propriedade {PropriedadeId}", id);
        var propriedade = await _propriedadeService.AtualizarAsync(id, GetProdutorId(), dto.Nome, dto.Descricao);
        
        if (propriedade == null)
        {
            _logger.LogWarning("Propriedade {PropriedadeId} não encontrada para atualização", id);
            return NotFound();
        }
        
        _logger.LogInformation("Propriedade {PropriedadeId} atualizada com sucesso", id);
        return Ok(propriedade);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        _logger.LogInformation("Excluindo propriedade {PropriedadeId}", id);
        var sucesso = await _propriedadeService.ExcluirAsync(id, GetProdutorId());
        
        if (!sucesso)
        {
            _logger.LogWarning("Propriedade {PropriedadeId} não encontrada para exclusão", id);
            return NotFound();
        }
        
        _logger.LogInformation("Propriedade {PropriedadeId} excluída com sucesso", id);
        return NoContent();
    }
}
