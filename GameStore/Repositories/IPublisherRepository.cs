using GameStore.Models;

namespace GameStore.Repositories;

public interface IPublisherRepository
{
    Task<Publisher?> GetByIdAsync(Guid id);

    Task<Publisher?> GetByIdTrackedAsync(Guid id);

    Task<Publisher?> GetByCompanyNameAsync(string companyName);

    Task<List<Publisher>> GetAllAsync();

    Task<Publisher?> GetByGameKeyAsync(string key);

    Task<bool> CompanyNameExistsAsync(string companyName, Guid? excludeId = null);

    Task AddAsync(Publisher publisher);

    Task DeleteAsync(Publisher publisher);
}
