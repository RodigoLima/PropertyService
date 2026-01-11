namespace PropertyService.Api.DTOs;

public record CriarTalhaoDto(string Nome, string Cultura, string? Descricao, decimal? AreaHectares);
