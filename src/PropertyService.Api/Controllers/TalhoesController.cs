using PropertyService.Api.DTOs;
using PropertyService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace PropertyService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TalhoesController : ControllerBase
{
    private readonly TalhaoService _talhaoService;

    public TalhoesController(TalhaoService talhaoService)
    {
        _talhaoService = talhaoService;
    }

    [HttpGet("propriedade/{propriedadeId}")]
    public async Task<IActionResult> ObterPorPropriedadeId(Guid propriedadeId)
    {
        var talhoes = await _talhaoService.ObterPorPropriedadeIdAsync(propriedadeId);
        return Ok(talhoes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var talhao = await _talhaoService.ObterPorIdAsync(id);
        if (talhao == null)
            return NotFound();

        return Ok(talhao);
    }

    [HttpPost("propriedade/{propriedadeId}")]
    public async Task<IActionResult> Criar(Guid propriedadeId, [FromBody] CriarTalhaoDto dto)
    {
        var talhao = await _talhaoService.CriarAsync(
            propriedadeId, 
            dto.Nome, 
            dto.Cultura, 
            dto.Descricao, 
            dto.AreaHectares);

        if (talhao == null)
            return BadRequest("Propriedade não encontrada.");

        return CreatedAtAction(nameof(ObterPorId), new { id = talhao.Id }, talhao);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarTalhaoDto dto)
    {
        var talhao = await _talhaoService.AtualizarAsync(
            id, 
            dto.Nome, 
            dto.Cultura, 
            dto.Descricao, 
            dto.AreaHectares);

        if (talhao == null)
            return NotFound();

        return Ok(talhao);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var sucesso = await _talhaoService.ExcluirAsync(id);
        if (!sucesso)
            return NotFound();

        return NoContent();
    }
}
