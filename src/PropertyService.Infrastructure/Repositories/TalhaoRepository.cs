using PropertyService.Application.Interfaces;
using PropertyService.Domain.Entities;
using PropertyService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PropertyService.Infrastructure.Repositories;

public class TalhaoRepository : ITalhaoRepository
{
    private readonly ApplicationDbContext _context;

    public TalhaoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Talhao?> ObterPorIdAsync(Guid id)
    {
        return await _context.Talhoes
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Talhao>> ObterPorPropriedadeIdAsync(Guid propriedadeId)
    {
        return await _context.Talhoes
            .Where(t => t.PropriedadeId == propriedadeId)
            .ToListAsync();
    }

    public async Task<Talhao> CriarAsync(Talhao talhao)
    {
        talhao.Id = Guid.NewGuid();
        talhao.DataCriacao = DateTime.UtcNow;
        
        _context.Talhoes.Add(talhao);
        await _context.SaveChangesAsync();
        
        // Limpar referência circular para evitar problemas na serialização
        _context.Entry(talhao).Reference(t => t.Propriedade).CurrentValue = null;
        
        return talhao;
    }

    public async Task<Talhao> AtualizarAsync(Talhao talhao)
    {
        _context.Talhoes.Update(talhao);
        await _context.SaveChangesAsync();
        
        return talhao;
    }

    public async Task<bool> ExcluirAsync(Guid id)
    {
        var talhao = await _context.Talhoes.FindAsync(id);
        if (talhao == null)
            return false;

        _context.Talhoes.Remove(talhao);
        await _context.SaveChangesAsync();
        
        return true;
    }
}
