using PropertyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PropertyService.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Propriedade> Propriedades { get; set; }
    public DbSet<Talhao> Talhoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da Propriedade
        modelBuilder.Entity<Propriedade>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descricao).HasMaxLength(1000);
            entity.Property(e => e.DataCriacao).IsRequired();
            
            entity.HasMany(e => e.Talhoes)
                  .WithOne(e => e.Propriedade)
                  .HasForeignKey(e => e.PropriedadeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração do Talhão
        modelBuilder.Entity<Talhao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descricao).HasMaxLength(1000);
            entity.Property(e => e.Cultura).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AreaHectares).HasPrecision(18, 2);
            entity.Property(e => e.DataCriacao).IsRequired();
            entity.Property(e => e.PropriedadeId).IsRequired();
        });
    }
}
