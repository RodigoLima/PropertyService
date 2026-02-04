using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PropertyService.Api.Configuration;
using PropertyService.Api.Services;
using System.Security.Claims;
using Moq;

namespace PropertyService.Tests.Services;

public class UserContextServiceTests
{
    private static readonly ILogger<UserContextService> Logger = new Mock<ILogger<UserContextService>>().Object;

    [Fact]
    public void GetProdutorId_QuandoBypassAtivo_DeveRetornarProdutorIdDev()
    {
        var identity = new ClaimsIdentity([], DevelopmentBypassConstants.SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var service = new UserContextService(httpContextAccessor, Logger);

        var resultado = service.GetProdutorId();

        resultado.Should().Be(DevelopmentBypassConstants.ProdutorId);
    }

    [Fact]
    public void GetProdutorId_QuandoClaimSubExiste_DeveRetornarGuid()
    {
        var produtorId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new("sub", produtorId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var service = new UserContextService(httpContextAccessor, Logger);

        // Act
        var resultado = service.GetProdutorId();

        // Assert
        resultado.Should().Be(produtorId);
    }

    [Fact]
    public void GetProdutorId_QuandoClaimUserIdExiste_DeveRetornarGuid()
    {
        // Arrange
        var produtorId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new("userId", produtorId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var service = new UserContextService(httpContextAccessor, Logger);

        var resultado = service.GetProdutorId();

        resultado.Should().Be(produtorId);
    }

    [Fact]
    public void GetProdutorId_QuandoClaimProdutorIdExiste_DeveRetornarGuid()
    {
        // Arrange
        var produtorId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new("produtorId", produtorId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var service = new UserContextService(httpContextAccessor, Logger);

        var resultado = service.GetProdutorId();

        resultado.Should().Be(produtorId);
    }

    [Fact]
    public void GetProdutorId_QuandoNenhumaClaimExiste_DeveLancarExcecao()
    {
        // Arrange
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var service = new UserContextService(httpContextAccessor, Logger);

        var act = () => service.GetProdutorId();
        act.Should().Throw<UnauthorizedAccessException>()
            .WithMessage("UserId não encontrado no token.");
    }

    [Fact]
    public void GetProdutorId_QuandoHttpContextENull_DeveLancarExcecao()
    {
        // Arrange
        var httpContextAccessor = new HttpContextAccessor { HttpContext = null };
        var service = new UserContextService(httpContextAccessor, Logger);

        var act = () => service.GetProdutorId();
        act.Should().Throw<UnauthorizedAccessException>()
            .WithMessage("Usuário não autenticado.");
    }

    [Fact]
    public void GetProdutorId_QuandoClaimTemValorInvalido_DeveLancarExcecao()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new("sub", "valor-invalido")
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var service = new UserContextService(httpContextAccessor, Logger);

        var act = () => service.GetProdutorId();
        act.Should().Throw<UnauthorizedAccessException>()
            .WithMessage("UserId não encontrado no token.");
    }

    [Fact]
    public void IsAuthenticated_QuandoUsuarioAutenticado_DeveRetornarTrue()
    {
        // Arrange
        var claims = new List<Claim> { new("sub", Guid.NewGuid().ToString()) };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var service = new UserContextService(httpContextAccessor, Logger);

        var resultado = service.IsAuthenticated;

        resultado.Should().BeTrue();
    }

    [Fact]
    public void IsAuthenticated_QuandoUsuarioNaoAutenticado_DeveRetornarFalse()
    {
        // Arrange
        var httpContextAccessor = new HttpContextAccessor { HttpContext = null };
        var service = new UserContextService(httpContextAccessor, Logger);

        var resultado = service.IsAuthenticated;

        resultado.Should().BeFalse();
    }
}
