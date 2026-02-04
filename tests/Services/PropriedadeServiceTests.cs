using FluentAssertions;
using MassTransit;
using Moq;
using PropertyService.Application.Interfaces;
using PropertyService.Application.Services;
using PropertyService.Domain.Entities;

namespace PropertyService.Tests.Services;

public class PropriedadeServiceTests
{
    private readonly Mock<IPropriedadeRepository> _repositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly PropriedadeService _service;

    public PropriedadeServiceTests()
    {
        _repositoryMock = new Mock<IPropriedadeRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _publishEndpointMock.Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var loggerMock = new Mock<ILogger<PropriedadeService>>();
        _service = new PropriedadeService(_repositoryMock.Object, _publishEndpointMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoPropriedadeExiste_DeveRetornarPropriedade()
    {
        // Arrange
        var id = Guid.NewGuid();
        var produtorId = Guid.NewGuid();
        var propriedade = new Propriedade
        {
            Id = id,
            ProdutorId = produtorId,
            Nome = "Fazenda Teste",
            Descricao = "Descrição teste"
        };

        _repositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(id, produtorId))
            .ReturnsAsync(propriedade);

        // Act
        var resultado = await _service.ObterPorIdAsync(id, produtorId);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(id);
        resultado.ProdutorId.Should().Be(produtorId);
        resultado.Nome.Should().Be("Fazenda Teste");
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoPropriedadeNaoExiste_DeveRetornarNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        var produtorId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(id, produtorId))
            .ReturnsAsync((Propriedade?)null);

        // Act
        var resultado = await _service.ObterPorIdAsync(id, produtorId);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterPorProdutorIdAsync_DeveRetornarListaDePropriedades()
    {
        // Arrange
        var produtorId = Guid.NewGuid();
        var propriedades = new List<Propriedade>
        {
            new() { Id = Guid.NewGuid(), ProdutorId = produtorId, Nome = "Fazenda 1" },
            new() { Id = Guid.NewGuid(), ProdutorId = produtorId, Nome = "Fazenda 2" }
        };

        _repositoryMock
            .Setup(r => r.ObterPorProdutorIdAsync(produtorId))
            .ReturnsAsync(propriedades);

        // Act
        var resultado = await _service.ObterPorProdutorIdAsync(produtorId);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().AllSatisfy(p => p.ProdutorId.Should().Be(produtorId));
    }

    [Fact]
    public async Task CriarAsync_DeveCriarPropriedadeComProdutorId()
    {
        // Arrange
        var produtorId = Guid.NewGuid();
        var propriedadeCriada = new Propriedade
        {
            Id = Guid.NewGuid(),
            ProdutorId = produtorId,
            Nome = "Nova Fazenda",
            Descricao = "Descrição",
            DataCriacao = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.CriarAsync(It.Is<Propriedade>(p => 
                p.ProdutorId == produtorId && 
                p.Nome == "Nova Fazenda")))
            .ReturnsAsync(propriedadeCriada);

        // Act
        var resultado = await _service.CriarAsync(produtorId, "Nova Fazenda", "Descrição");

        // Assert
        resultado.Should().NotBeNull();
        resultado.ProdutorId.Should().Be(produtorId);
        resultado.Nome.Should().Be("Nova Fazenda");
        _repositoryMock.Verify(r => r.CriarAsync(It.IsAny<Propriedade>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoPropriedadeExiste_DeveAtualizar()
    {
        // Arrange
        var id = Guid.NewGuid();
        var produtorId = Guid.NewGuid();
        var propriedade = new Propriedade
        {
            Id = id,
            ProdutorId = produtorId,
            Nome = "Nome Antigo",
            Descricao = "Descrição Antiga"
        };

        _repositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(id, produtorId))
            .ReturnsAsync(propriedade);

        _repositoryMock
            .Setup(r => r.AtualizarAsync(It.IsAny<Propriedade>()))
            .ReturnsAsync(propriedade);

        // Act
        var resultado = await _service.AtualizarAsync(id, produtorId, "Nome Novo", "Descrição Nova");

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Nome.Should().Be("Nome Novo");
        resultado.Descricao.Should().Be("Descrição Nova");
        _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Propriedade>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoPropriedadeNaoExiste_DeveRetornarNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        var produtorId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(id, produtorId))
            .ReturnsAsync((Propriedade?)null);

        // Act
        var resultado = await _service.AtualizarAsync(id, produtorId, "Nome Novo", "Descrição Nova");

        // Assert
        resultado.Should().BeNull();
        _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Propriedade>()), Times.Never);
    }

    [Fact]
    public async Task ExcluirAsync_QuandoPropriedadeExiste_DeveExcluir()
    {
        // Arrange
        var id = Guid.NewGuid();
        var produtorId = Guid.NewGuid();
        var propriedade = new Propriedade { Id = id, ProdutorId = produtorId };

        _repositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(id, produtorId))
            .ReturnsAsync(propriedade);

        _repositoryMock
            .Setup(r => r.ExcluirAsync(id))
            .ReturnsAsync(true);

        // Act
        var resultado = await _service.ExcluirAsync(id, produtorId);

        // Assert
        resultado.Should().BeTrue();
        _repositoryMock.Verify(r => r.ExcluirAsync(id), Times.Once);
    }

    [Fact]
    public async Task ExcluirAsync_QuandoPropriedadeNaoExiste_DeveRetornarFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var produtorId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(id, produtorId))
            .ReturnsAsync((Propriedade?)null);

        // Act
        var resultado = await _service.ExcluirAsync(id, produtorId);

        // Assert
        resultado.Should().BeFalse();
        _repositoryMock.Verify(r => r.ExcluirAsync(It.IsAny<Guid>()), Times.Never);
    }
}
