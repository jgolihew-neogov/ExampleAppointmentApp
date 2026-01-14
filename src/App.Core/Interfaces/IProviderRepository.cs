using App.Core.Entities;

namespace App.Core.Interfaces;

public interface IProviderRepository
{
    Task<Provider?> GetByIdAsync(Guid id);
    Task<List<Provider>> GetAllAsync();
    Task AddAsync(Provider provider);
    Task SaveChangesAsync();
}
