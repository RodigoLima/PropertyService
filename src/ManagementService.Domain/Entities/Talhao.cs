namespace ManagementService.Domain.Entities;

public class Talhao
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Cultura { get; set; } = string.Empty; // Cultura plantada
    public decimal? AreaHectares { get; set; } // Área em hectares (opcional)
    public DateTime DataCriacao { get; set; }
    
    // Relacionamento: Um talhão pertence a uma propriedade
    public Guid PropriedadeId { get; set; }
    public Propriedade Propriedade { get; set; } = null!;
}
