using App.Core.Entities;
using App.Core.Interfaces;

namespace App.Core.Services;

public class ProviderService
{
    private readonly IProviderRepository _providerRepository;

    public ProviderService(IProviderRepository providerRepository)
    {
        _providerRepository = providerRepository;
    }

    public async Task<Result<Provider>> CreateProviderAsync(string name, string? timeZone = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Provider>.Failure("Provider name is required");

        var provider = new Provider
        {
            Id = Guid.NewGuid(),
            Name = name,
            TimeZone = timeZone ?? "UTC",
            IsActive = true
        };

        await _providerRepository.AddAsync(provider);
        await _providerRepository.SaveChangesAsync();

        return Result<Provider>.Success(provider);
    }

    public async Task<List<Provider>> GetAllProvidersAsync()
    {
        return await _providerRepository.GetAllAsync();
    }
}
