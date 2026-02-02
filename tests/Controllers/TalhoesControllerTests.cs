using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PropertyService.Api.Controllers;
using PropertyService.Api.DTOs;
using PropertyService.Application.Interfaces;
using PropertyService.Application.Services;
using PropertyService.Domain.Entities;

namespace PropertyService.Tests.Controllers;

public class TalhoesControllerTests
{
    private readonly Mock<ITalhaoRepository> _talhaoRepositoryMock;
    private readonly Mock<IPropriedadeRepository> _propriedadeRepositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly TalhaoService _service;
    private readonly Mock<IUserContextService> _userContextMock;
    private readonly TalhoesController _controller;

    public TalhoesControllerTests()
    {
        _talhaoRepositoryMock = new Mock<ITalhaoRepository>();
        _propriedadeRepositoryMock = new Mock<IPropriedadeRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _publishEndpointMock.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _service = new TalhaoService(_talhaoRepositoryMock.Object, _propriedadeRepositoryMock.Object, _publishEndpointMock.Object);
        _userContextMock = new Mock<IUserContextService>();
        _controller = new TalhoesController(_service, _userContextMock.Object);
    }

    [Fact]
    public async Task ObterPorPropriedadeId_QuandoPropriedadeExiste_DeveRetornarOk()
    {
        // Arrange
        var propriedadeId = Guid.NewGuid();
        var produtorId = Guid.NewGuid();
        var propriedade = new Propriedade { Id = propriedadeId, ProdutorId = produtorId };
        var talhoes = new List<Talhao>
        {
            new() { Id = Guid.NewGuid(), PropriedadeId = propriedadeId, Nome = "Talhão 1" }
        };

        _userContextMock.Setup(u => u.GetProdutorId()).Returns(produtorId);
        _propriedadeRepositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(propriedadeId, produtorId))
            .ReturnsAsync(propriedade);
        _talhaoRepositoryMock
            .Setup(r => r.ObterPorPropriedadeIdAsync(propriedadeId))
            .ReturnsAsync(talhoes);

        // Act
        var resultado = await _controller.ObterPorPropriedadeId(propriedadeId);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(talhoes);
    }

    [Fact]
    public async Task ObterPorId_QuandoTalhaoExiste_DeveRetornarOk()
    {
        // Arrange
        var talhaoId = Guid.NewGuid();
        var propriedadeId = Guid.NewGuid();
        var produtorId = Guid.NewGuid();
        var talhao = new Talhao { Id = talhaoId, PropriedadeId = propriedadeId };
        var propriedade = new Propriedade { Id = propriedadeId, ProdutorId = produtorId };

        _userContextMock.Setup(u => u.GetProdutorId()).Returns(produtorId);
        _talhaoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(talhaoId))
            .ReturnsAsync(talhao);
        _propriedadeRepositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(propriedadeId, produtorId))
            .ReturnsAsync(propriedade);

        // Act
        var resultado = await _controller.ObterPorId(talhaoId);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(talhao);
    }

    [Fact]
    public async Task ObterPorId_QuandoTalhaoNaoExiste_DeveRetornarNotFound()
    {
        // Arrange
        var talhaoId = Guid.NewGuid();
        var produtorId = Guid.NewGuid();

        _userContextMock.Setup(u => u.GetProdutorId()).Returns(produtorId);
        _talhaoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(talhaoId))
            .ReturnsAsync((Talhao?)null);

        // Act
        var resultado = await _controller.ObterPorId(talhaoId);

        // Assert
        resultado.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Criar_QuandoPropriedadeExiste_DeveRetornarCreatedAtAction()
    {
        // Arrange
        var propriedadeId = Guid.NewGuid();
        var produtorId = Guid.NewGuid();
        var talhaoId = Guid.NewGuid();
        var dto = new CriarTalhaoDto("Novo Talhão", "Soja", "Descrição", 10.5m);
        var propriedade = new Propriedade { Id = propriedadeId, ProdutorId = produtorId };
        var talhao = new Talhao
        {
            Id = talhaoId,
            PropriedadeId = propriedadeId,
            Nome = dto.Nome,
            Cultura = dto.Cultura
        };

        _userContextMock.Setup(u => u.GetProdutorId()).Returns(produtorId);
        _propriedadeRepositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(propriedadeId, produtorId))
            .ReturnsAsync(propriedade);
        _talhaoRepositoryMock
            .Setup(r => r.CriarAsync(It.Is<Talhao>(t => t.PropriedadeId == propriedadeId && t.Nome == dto.Nome)))
            .ReturnsAsync(talhao);

        // Act
        var resultado = await _controller.Criar(propriedadeId, dto);

        // Assert
        resultado.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = resultado as CreatedAtActionResult;
        createdResult!.Value.Should().BeEquivalentTo(talhao);
    }

    [Fact]
    public async Task Criar_QuandoPropriedadeNaoExiste_DeveRetornarBadRequest()
    {
        // Arrange
        var propriedadeId = Guid.NewGuid();
        var produtorId = Guid.NewGuid();
        var dto = new CriarTalhaoDto("Novo Talhão", "Soja", null, null);

        _userContextMock.Setup(u => u.GetProdutorId()).Returns(produtorId);
        _propriedadeRepositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(propriedadeId, produtorId))
            .ReturnsAsync((Propriedade?)null);

        // Act
        var resultado = await _controller.Criar(propriedadeId, dto);

        // Assert
        resultado.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = resultado as BadRequestObjectResult;
        badRequest!.Value.Should().Be("Propriedade não encontrada ou não pertence ao produtor.");
    }

    [Fact]
    public async Task Atualizar_QuandoTalhaoExiste_DeveRetornarOk()
    {
        // Arrange
        var talhaoId = Guid.NewGuid();
        var propriedadeId = Guid.NewGuid();
        var produtorId = Guid.NewGuid();
        var dto = new AtualizarTalhaoDto("Nome Atualizado", "Milho", "Descrição", 15.0m);
        var talhao = new Talhao { Id = talhaoId, PropriedadeId = propriedadeId };
        var propriedade = new Propriedade { Id = propriedadeId, ProdutorId = produtorId };

        _userContextMock.Setup(u => u.GetProdutorId()).Returns(produtorId);
        _talhaoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(talhaoId))
            .ReturnsAsync(talhao);
        _propriedadeRepositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(propriedadeId, produtorId))
            .ReturnsAsync(propriedade);
        _talhaoRepositoryMock
            .Setup(r => r.AtualizarAsync(It.IsAny<Talhao>()))
            .ReturnsAsync(talhao);

        // Act
        var resultado = await _controller.Atualizar(talhaoId, dto);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Excluir_QuandoTalhaoExiste_DeveRetornarNoContent()
    {
        // Arrange
        var talhaoId = Guid.NewGuid();
        var propriedadeId = Guid.NewGuid();
        var produtorId = Guid.NewGuid();
        var talhao = new Talhao { Id = talhaoId, PropriedadeId = propriedadeId };
        var propriedade = new Propriedade { Id = propriedadeId, ProdutorId = produtorId };

        _userContextMock.Setup(u => u.GetProdutorId()).Returns(produtorId);
        _talhaoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(talhaoId))
            .ReturnsAsync(talhao);
        _propriedadeRepositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(propriedadeId, produtorId))
            .ReturnsAsync(propriedade);
        _talhaoRepositoryMock
            .Setup(r => r.ExcluirAsync(talhaoId))
            .ReturnsAsync(true);

        // Act
        var resultado = await _controller.Excluir(talhaoId);

        // Assert
        resultado.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Excluir_QuandoTalhaoNaoExiste_DeveRetornarNotFound()
    {
        // Arrange
        var talhaoId = Guid.NewGuid();
        var produtorId = Guid.NewGuid();

        _userContextMock.Setup(u => u.GetProdutorId()).Returns(produtorId);
        _talhaoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(talhaoId))
            .ReturnsAsync((Talhao?)null);

        // Act
        var resultado = await _controller.Excluir(talhaoId);

        // Assert
        resultado.Should().BeOfType<NotFoundResult>();
    }
}
