using GameStore.Data;
using GameStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Repositories;

public class PublisherRepository(GameStoreDbContext dbContext) : IPublisherRepository
{
    private readonly GameStoreDbContext _dbContext = dbContext;

    public Task<Publisher?> GetByIdAsync(Guid id)
    {
        return _dbContext.Publishers.AsNoTracking().FirstOrDefaultAsync(publisher => publisher.Id == id);
    }

    public Task<Publisher?> GetByIdTrackedAsync(Guid id)
    {
        return _dbContext.Publishers.FirstOrDefaultAsync(g => g.Id == id);
    }

    public Task<Publisher?> GetByCompanyNameAsync(string companyName)
    {
        return _dbContext.Publishers.AsNoTracking().FirstOrDefaultAsync(publisher => publisher.CompanyName == companyName);
    }

    public Task<List<Publisher>> GetAllAsync()
    {
        return _dbContext.Publishers.AsNoTracking().ToListAsync();
    }

    public Task<Publisher?> GetByGameKeyAsync(string key)
    {
        return _dbContext.Games.AsNoTracking()
            .Where(game => game.Key == key)
            .Select(game => game.Publisher)
            .FirstOrDefaultAsync();
    }

    public Task<bool> CompanyNameExistsAsync(string companyName, Guid? excludeId = null)
    {
        var query = _dbContext.Publishers.AsNoTracking().Where(publisher => publisher.CompanyName == companyName);
        if (excludeId.HasValue)
        {
            query = query.Where(publisher => publisher.Id != excludeId.Value);
        }

        return query.AnyAsync();
    }

    public Task AddAsync(Publisher publisher)
    {
        _dbContext.Publishers.Add(publisher);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Publisher publisher)
    {
        _dbContext.Publishers.Remove(publisher);
        return Task.CompletedTask;
    }
}
