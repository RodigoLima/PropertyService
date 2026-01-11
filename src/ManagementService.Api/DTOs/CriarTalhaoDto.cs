namespace ManagementService.Api.DTOs;

public record CriarTalhaoDto(string Nome, string Cultura, string? Descricao, decimal? AreaHectares);
