using PropertyService.Application.Interfaces;
using PropertyService.Domain.Entities;
using PropertyService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PropertyService.Infrastructure.Repositories;

public class PropriedadeRepository : IPropriedadeRepository
{
    private readonly ApplicationDbContext _context;

    public PropriedadeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Propriedade?> ObterPorIdAsync(Guid id)
    {
        return await _context.Propriedades
            .Include(p => p.Talhoes)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Propriedade?> ObterPorIdEProdutorIdAsync(Guid id, Guid produtorId)
    {
        return await _context.Propriedades
            .Include(p => p.Talhoes)
            .FirstOrDefaultAsync(p => p.Id == id && p.ProdutorId == produtorId);
    }

    public async Task<IEnumerable<Propriedade>> ObterTodasAsync()
    {
        return await _context.Propriedades
            .Include(p => p.Talhoes)
            .ToListAsync();
    }

    public async Task<IEnumerable<Propriedade>> ObterPorProdutorIdAsync(Guid produtorId)
    {
        return await _context.Propriedades
            .Include(p => p.Talhoes)
            .Where(p => p.ProdutorId == produtorId)
            .ToListAsync();
    }

    public async Task<Propriedade> CriarAsync(Propriedade propriedade)
    {
        propriedade.Id = Guid.NewGuid();
        propriedade.DataCriacao = DateTime.UtcNow;
        
        _context.Propriedades.Add(propriedade);
        await _context.SaveChangesAsync();
        
        return propriedade;
    }

    public async Task<Propriedade> AtualizarAsync(Propriedade propriedade)
    {
        _context.Propriedades.Update(propriedade);
        await _context.SaveChangesAsync();
        
        return propriedade;
    }

    public async Task<bool> ExcluirAsync(Guid id)
    {
        var propriedade = await _context.Propriedades.FindAsync(id);
        if (propriedade == null)
            return false;

        _context.Propriedades.Remove(propriedade);
        await _context.SaveChangesAsync();
        
        return true;
    }
}
