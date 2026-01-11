using PropertyService.Api.DTOs;
using PropertyService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace PropertyService.Api.Controllers;

[Route("api/[controller]")]
public class PropriedadesController : BaseController
{
    private readonly PropriedadeService _propriedadeService;

    public PropriedadesController(PropriedadeService propriedadeService, IUserContextService userContext)
        : base(userContext)
    {
        _propriedadeService = propriedadeService;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodas()
    {
        var propriedades = await _propriedadeService.ObterPorProdutorIdAsync(GetProdutorId());
        return Ok(propriedades);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var propriedade = await _propriedadeService.ObterPorIdAsync(id, GetProdutorId());
        return propriedade == null ? NotFound() : Ok(propriedade);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarPropriedadeDto dto)
    {
        var propriedade = await _propriedadeService.CriarAsync(GetProdutorId(), dto.Nome, dto.Descricao);
        return CreatedAtAction(nameof(ObterPorId), new { id = propriedade.Id }, propriedade);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarPropriedadeDto dto)
    {
        var propriedade = await _propriedadeService.AtualizarAsync(id, GetProdutorId(), dto.Nome, dto.Descricao);
        return propriedade == null ? NotFound() : Ok(propriedade);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var sucesso = await _propriedadeService.ExcluirAsync(id, GetProdutorId());
        return sucesso ? NoContent() : NotFound();
    }
}
