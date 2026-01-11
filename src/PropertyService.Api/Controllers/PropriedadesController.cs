using PropertyService.Api.DTOs;
using PropertyService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace PropertyService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropriedadesController : ControllerBase
{
    private readonly PropriedadeService _propriedadeService;

    public PropriedadesController(PropriedadeService propriedadeService)
    {
        _propriedadeService = propriedadeService;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodas()
    {
        var propriedades = await _propriedadeService.ObterTodasAsync();
        return Ok(propriedades);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var propriedade = await _propriedadeService.ObterPorIdAsync(id);
        if (propriedade == null)
            return NotFound();

        return Ok(propriedade);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarPropriedadeDto dto)
    {
        var propriedade = await _propriedadeService.CriarAsync(dto.Nome, dto.Descricao);
        return CreatedAtAction(nameof(ObterPorId), new { id = propriedade.Id }, propriedade);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarPropriedadeDto dto)
    {
        var propriedade = await _propriedadeService.AtualizarAsync(id, dto.Nome, dto.Descricao);
        if (propriedade == null)
            return NotFound();

        return Ok(propriedade);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var sucesso = await _propriedadeService.ExcluirAsync(id);
        if (!sucesso)
            return NotFound();

        return NoContent();
    }
}
