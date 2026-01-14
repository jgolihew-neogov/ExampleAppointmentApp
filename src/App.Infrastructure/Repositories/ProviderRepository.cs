using App.Core.Entities;
using App.Core.Interfaces;
using App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repositories;

public class ProviderRepository : IProviderRepository
{
    private readonly AppDbContext _context;

    public ProviderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Provider?> GetByIdAsync(Guid id)
    {
        return await _context.Providers.FindAsync(id);
    }

    public async Task<List<Provider>> GetAllAsync()
    {
        return await _context.Providers
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task AddAsync(Provider provider)
    {
        await _context.Providers.AddAsync(provider);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
