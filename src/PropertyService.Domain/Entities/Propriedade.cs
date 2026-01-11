namespace PropertyService.Domain.Entities;

public class Propriedade
{
    public Guid Id { get; set; }
    public Guid ProdutorId { get; set; } // ID do produtor/usuário que possui a propriedade
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataCriacao { get; set; }
    
    // Relacionamento: Uma propriedade tem vários talhões
    public ICollection<Talhao> Talhoes { get; set; } = new List<Talhao>();
}
