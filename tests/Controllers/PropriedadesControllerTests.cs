using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PropertyService.Api.Controllers;
using PropertyService.Api.DTOs;
using PropertyService.Application.Interfaces;
using PropertyService.Application.Services;
using PropertyService.Domain.Entities;

namespace PropertyService.Tests.Controllers;

public class PropriedadesControllerTests
{
    private readonly Mock<IPropriedadeRepository> _repositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly PropriedadeService _service;
    private readonly Mock<IUserContextService> _userContextMock;
    private readonly Mock<ILogger<PropriedadesController>> _loggerMock;
    private readonly PropriedadesController _controller;

    public PropriedadesControllerTests()
    {
        _repositoryMock = new Mock<IPropriedadeRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _publishEndpointMock.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _service = new PropriedadeService(_repositoryMock.Object, _publishEndpointMock.Object);
        _userContextMock = new Mock<IUserContextService>();
        _loggerMock = new Mock<ILogger<PropriedadesController>>();
        _controller = new PropriedadesController(_service, _userContextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ObterTodas_DeveRetornarOkComListaDePropriedades()
    {
        // Arrange
        var produtorId = Guid.NewGuid();
        var propriedades = new List<Propriedade>
        {
            new() { Id = Guid.NewGuid(), ProdutorId = produtorId, Nome = "Fazenda 1" },
            new() { Id = Guid.NewGuid(), ProdutorId = produtorId, Nome = "Fazenda 2" }
        };

        _userContextMock.Setup(u => u.GetProdutorId()).Returns(produtorId);
        _repositoryMock
            .Setup(r => r.ObterPorProdutorIdAsync(produtorId))
            .ReturnsAsync(propriedades);

        // Act
        var resultado = await _controller.ObterTodas();

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(propriedades);
    }

    [Fact]
    public async Task ObterPorId_QuandoPropriedadeExiste_DeveRetornarOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var produtorId = Guid.NewGuid();
        var propriedade = new Propriedade
        {
            Id = id,
            ProdutorId = produtorId,
            Nome = "Fazenda Teste"
        };

        _userContextMock.Setup(u => u.GetProdutorId()).Returns(produtorId);
        _repositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(id, produtorId))
            .ReturnsAsync(propriedade);

        // Act
        var resultado = await _controller.ObterPorId(id);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(propriedade);
    }

    [Fact]
    public async Task ObterPorId_QuandoPropriedadeNaoExiste_DeveRetornarNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var produtorId = Guid.NewGuid();

        _userContextMock.Setup(u => u.GetProdutorId()).Returns(produtorId);
        _repositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(id, produtorId))
            .ReturnsAsync((Propriedade?)null);

        // Act
        var resultado = await _controller.ObterPorId(id);

        // Assert
        resultado.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Criar_DeveRetornarCreatedAtAction()
    {
        // Arrange
        var produtorId = Guid.NewGuid();
        var propriedadeId = Guid.NewGuid();
        var dto = new CriarPropriedadeDto("Nova Fazenda", "Descrição");
        var propriedade = new Propriedade
        {
            Id = propriedadeId,
            ProdutorId = produtorId,
            Nome = dto.Nome,
            Descricao = dto.Descricao
        };

        _userContextMock.Setup(u => u.GetProdutorId()).Returns(produtorId);
        _repositoryMock
            .Setup(r => r.CriarAsync(It.Is<Propriedade>(p => p.ProdutorId == produtorId && p.Nome == dto.Nome)))
            .ReturnsAsync(propriedade);

        // Act
        var resultado = await _controller.Criar(dto);

        // Assert
        resultado.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = resultado as CreatedAtActionResult;
        createdResult!.Value.Should().BeEquivalentTo(propriedade);
        createdResult.ActionName.Should().Be(nameof(_controller.ObterPorId));
    }

    [Fact]
    public async Task Atualizar_QuandoPropriedadeExiste_DeveRetornarOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var produtorId = Guid.NewGuid();
        var dto = new AtualizarPropriedadeDto("Nome Atualizado", "Descrição Atualizada");
        var propriedade = new Propriedade
        {
            Id = id,
            ProdutorId = produtorId,
            Nome = dto.Nome,
            Descricao = dto.Descricao
        };

        _userContextMock.Setup(u => u.GetProdutorId()).Returns(produtorId);
        _repositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(id, produtorId))
            .ReturnsAsync(propriedade);
        _repositoryMock
            .Setup(r => r.AtualizarAsync(It.IsAny<Propriedade>()))
            .ReturnsAsync(propriedade);

        // Act
        var resultado = await _controller.Atualizar(id, dto);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(propriedade);
    }

    [Fact]
    public async Task Atualizar_QuandoPropriedadeNaoExiste_DeveRetornarNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var produtorId = Guid.NewGuid();
        var dto = new AtualizarPropriedadeDto("Nome", "Descrição");

        _userContextMock.Setup(u => u.GetProdutorId()).Returns(produtorId);
        _repositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(id, produtorId))
            .ReturnsAsync((Propriedade?)null);

        // Act
        var resultado = await _controller.Atualizar(id, dto);

        // Assert
        resultado.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Excluir_QuandoPropriedadeExiste_DeveRetornarNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var produtorId = Guid.NewGuid();

        var propriedade = new Propriedade { Id = id, ProdutorId = produtorId };
        _userContextMock.Setup(u => u.GetProdutorId()).Returns(produtorId);
        _repositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(id, produtorId))
            .ReturnsAsync(propriedade);
        _repositoryMock
            .Setup(r => r.ExcluirAsync(id))
            .ReturnsAsync(true);

        // Act
        var resultado = await _controller.Excluir(id);

        // Assert
        resultado.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Excluir_QuandoPropriedadeNaoExiste_DeveRetornarNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var produtorId = Guid.NewGuid();

        _userContextMock.Setup(u => u.GetProdutorId()).Returns(produtorId);
        _repositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(id, produtorId))
            .ReturnsAsync((Propriedade?)null);

        // Act
        var resultado = await _controller.Excluir(id);

        // Assert
        resultado.Should().BeOfType<NotFoundResult>();
    }
}
