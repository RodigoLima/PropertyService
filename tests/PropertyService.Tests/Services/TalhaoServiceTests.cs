using FluentAssertions;
using Moq;
using PropertyService.Application.Interfaces;
using PropertyService.Application.Services;
using PropertyService.Domain.Entities;

namespace PropertyService.Tests.Services;

public class TalhaoServiceTests
{
    private readonly Mock<ITalhaoRepository> _talhaoRepositoryMock;
    private readonly Mock<IPropriedadeRepository> _propriedadeRepositoryMock;
    private readonly TalhaoService _service;

    public TalhaoServiceTests()
    {
        _talhaoRepositoryMock = new Mock<ITalhaoRepository>();
        _propriedadeRepositoryMock = new Mock<IPropriedadeRepository>();
        _service = new TalhaoService(_talhaoRepositoryMock.Object, _propriedadeRepositoryMock.Object);
    }

    [Fact]
    public async Task ObterPorIdEProdutorIdAsync_QuandoTalhaoExisteEPertenceAoProdutor_DeveRetornarTalhao()
    {
        // Arrange
        var talhaoId = Guid.NewGuid();
        var propriedadeId = Guid.NewGuid();
        var produtorId = Guid.NewGuid();

        var talhao = new Talhao
        {
            Id = talhaoId,
            PropriedadeId = propriedadeId,
            Nome = "Talhão 1",
            Cultura = "Soja"
        };

        var propriedade = new Propriedade
        {
            Id = propriedadeId,
            ProdutorId = produtorId
        };

        _talhaoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(talhaoId))
            .ReturnsAsync(talhao);

        _propriedadeRepositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(propriedadeId, produtorId))
            .ReturnsAsync(propriedade);

        // Act
        var resultado = await _service.ObterPorIdEProdutorIdAsync(talhaoId, produtorId);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(talhaoId);
    }

    [Fact]
    public async Task ObterPorIdEProdutorIdAsync_QuandoTalhaoNaoExiste_DeveRetornarNull()
    {
        // Arrange
        var talhaoId = Guid.NewGuid();
        var produtorId = Guid.NewGuid();

        _talhaoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(talhaoId))
            .ReturnsAsync((Talhao?)null);

        // Act
        var resultado = await _service.ObterPorIdEProdutorIdAsync(talhaoId, produtorId);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterPorIdEProdutorIdAsync_QuandoPropriedadeNaoPertenceAoProdutor_DeveRetornarNull()
    {
        // Arrange
        var talhaoId = Guid.NewGuid();
        var propriedadeId = Guid.NewGuid();
        var produtorId = Guid.NewGuid();

        var talhao = new Talhao
        {
            Id = talhaoId,
            PropriedadeId = propriedadeId
        };

        _talhaoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(talhaoId))
            .ReturnsAsync(talhao);

        _propriedadeRepositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(propriedadeId, produtorId))
            .ReturnsAsync((Propriedade?)null);

        // Act
        var resultado = await _service.ObterPorIdEProdutorIdAsync(talhaoId, produtorId);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterPorPropriedadeIdEProdutorIdAsync_QuandoPropriedadeExiste_DeveRetornarTalhoes()
    {
        // Arrange
        var propriedadeId = Guid.NewGuid();
        var produtorId = Guid.NewGuid();

        var propriedade = new Propriedade
        {
            Id = propriedadeId,
            ProdutorId = produtorId
        };

        var talhoes = new List<Talhao>
        {
            new() { Id = Guid.NewGuid(), PropriedadeId = propriedadeId, Nome = "Talhão 1" },
            new() { Id = Guid.NewGuid(), PropriedadeId = propriedadeId, Nome = "Talhão 2" }
        };

        _propriedadeRepositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(propriedadeId, produtorId))
            .ReturnsAsync(propriedade);

        _talhaoRepositoryMock
            .Setup(r => r.ObterPorPropriedadeIdAsync(propriedadeId))
            .ReturnsAsync(talhoes);

        // Act
        var resultado = await _service.ObterPorPropriedadeIdEProdutorIdAsync(propriedadeId, produtorId);

        // Assert
        resultado.Should().HaveCount(2);
    }

    [Fact]
    public async Task ObterPorPropriedadeIdEProdutorIdAsync_QuandoPropriedadeNaoExiste_DeveRetornarVazio()
    {
        // Arrange
        var propriedadeId = Guid.NewGuid();
        var produtorId = Guid.NewGuid();

        _propriedadeRepositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(propriedadeId, produtorId))
            .ReturnsAsync((Propriedade?)null);

        // Act
        var resultado = await _service.ObterPorPropriedadeIdEProdutorIdAsync(propriedadeId, produtorId);

        // Assert
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task CriarAsync_QuandoPropriedadeExiste_DeveCriarTalhao()
    {
        // Arrange
        var propriedadeId = Guid.NewGuid();
        var produtorId = Guid.NewGuid();

        var propriedade = new Propriedade
        {
            Id = propriedadeId,
            ProdutorId = produtorId
        };

        var talhaoCriado = new Talhao
        {
            Id = Guid.NewGuid(),
            PropriedadeId = propriedadeId,
            Nome = "Novo Talhão",
            Cultura = "Milho"
        };

        _propriedadeRepositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(propriedadeId, produtorId))
            .ReturnsAsync(propriedade);

        _talhaoRepositoryMock
            .Setup(r => r.CriarAsync(It.Is<Talhao>(t => 
                t.PropriedadeId == propriedadeId && 
                t.Nome == "Novo Talhão")))
            .ReturnsAsync(talhaoCriado);

        // Act
        var resultado = await _service.CriarAsync(propriedadeId, produtorId, "Novo Talhão", "Milho");

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Nome.Should().Be("Novo Talhão");
        resultado.Cultura.Should().Be("Milho");
        _talhaoRepositoryMock.Verify(r => r.CriarAsync(It.IsAny<Talhao>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_QuandoPropriedadeNaoExiste_DeveRetornarNull()
    {
        // Arrange
        var propriedadeId = Guid.NewGuid();
        var produtorId = Guid.NewGuid();

        _propriedadeRepositoryMock
            .Setup(r => r.ObterPorIdEProdutorIdAsync(propriedadeId, produtorId))
            .ReturnsAsync((Propriedade?)null);

        // Act
        var resultado = await _service.CriarAsync(propriedadeId, produtorId, "Novo Talhão", "Milho");

        // Assert
        resultado.Should().BeNull();
        _talhaoRepositoryMock.Verify(r => r.CriarAsync(It.IsAny<Talhao>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_QuandoTalhaoExiste_DeveAtualizar()
    {
        // Arrange
        var talhaoId = Guid.NewGuid();
        var propriedadeId = Guid.NewGuid();
        var produtorId = Guid.NewGuid();

        var talhao = new Talhao
        {
            Id = talhaoId,
            PropriedadeId = propriedadeId,
            Nome = "Nome Antigo",
            Cultura = "Soja"
        };

        var propriedade = new Propriedade
        {
            Id = propriedadeId,
            ProdutorId = produtorId
        };

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
        var resultado = await _service.AtualizarAsync(talhaoId, produtorId, "Nome Novo", "Milho");

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Nome.Should().Be("Nome Novo");
        resultado.Cultura.Should().Be("Milho");
        _talhaoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Talhao>()), Times.Once);
    }

    [Fact]
    public async Task ExcluirAsync_QuandoTalhaoExiste_DeveExcluir()
    {
        // Arrange
        var talhaoId = Guid.NewGuid();
        var propriedadeId = Guid.NewGuid();
        var produtorId = Guid.NewGuid();

        var talhao = new Talhao
        {
            Id = talhaoId,
            PropriedadeId = propriedadeId
        };

        var propriedade = new Propriedade
        {
            Id = propriedadeId,
            ProdutorId = produtorId
        };

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
        var resultado = await _service.ExcluirAsync(talhaoId, produtorId);

        // Assert
        resultado.Should().BeTrue();
        _talhaoRepositoryMock.Verify(r => r.ExcluirAsync(talhaoId), Times.Once);
    }

    [Fact]
    public async Task ExcluirAsync_QuandoTalhaoNaoExiste_DeveRetornarFalse()
    {
        // Arrange
        var talhaoId = Guid.NewGuid();
        var produtorId = Guid.NewGuid();

        _talhaoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(talhaoId))
            .ReturnsAsync((Talhao?)null);

        // Act
        var resultado = await _service.ExcluirAsync(talhaoId, produtorId);

        // Assert
        resultado.Should().BeFalse();
        _talhaoRepositoryMock.Verify(r => r.ExcluirAsync(It.IsAny<Guid>()), Times.Never);
    }
}
