using Microsoft.AspNetCore.Mvc;
using PropertyService.Api.DTOs;
using PropertyService.Application.Services;

namespace PropertyService.Api.Controllers;

[Route("api/[controller]")]
public class TalhoesController : BaseController
{
    private readonly TalhaoService _talhaoService;

    public TalhoesController(TalhaoService talhaoService, IUserContextService userContext)
        : base(userContext)
    {
        _talhaoService = talhaoService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var talhao = await _talhaoService.ObterPorIdEProdutorIdAsync(id, GetProdutorId());
        return talhao == null ? NotFound() : Ok(talhao);
    }

    [HttpGet("propriedade/{propriedadeId}")]
    public async Task<IActionResult> ObterPorPropriedadeId(Guid propriedadeId)
    {
        var talhoes = await _talhaoService.ObterPorPropriedadeIdEProdutorIdAsync(propriedadeId, GetProdutorId());
        return Ok(talhoes);
    }

    [HttpPost("propriedade/{propriedadeId}")]
    public async Task<IActionResult> Criar(Guid propriedadeId, [FromBody] CriarTalhaoDto dto)
    {
        var talhao = await _talhaoService.CriarAsync(
            propriedadeId,
            GetProdutorId(),
            dto.Nome, 
            dto.Cultura, 
            dto.Descricao, 
            dto.AreaHectares);

        return talhao == null 
            ? BadRequest("Propriedade não encontrada ou não pertence ao produtor.") 
            : CreatedAtAction(nameof(ObterPorId), new { id = talhao.Id }, talhao);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarTalhaoDto dto)
    {
        var talhao = await _talhaoService.AtualizarAsync(
            id,
            GetProdutorId(),
            dto.Nome, 
            dto.Cultura, 
            dto.Descricao, 
            dto.AreaHectares);

        return talhao == null ? NotFound() : Ok(talhao);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var sucesso = await _talhaoService.ExcluirAsync(id, GetProdutorId());
        return sucesso ? NoContent() : NotFound();
    }
}
