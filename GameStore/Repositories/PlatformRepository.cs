using GameStore.Data;
using GameStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Repositories;

public class PlatformRepository(GameStoreDbContext dbContext) : IPlatformRepository
{
    private readonly GameStoreDbContext _dbContext = dbContext;

    public Task<bool> ExistsAsync(Guid id)
    {
        return _dbContext.Platforms.AnyAsync(platform => platform.Id == id);
    }

    public Task<Platform?> GetByIdAsync(Guid id)
    {
        return _dbContext.Platforms.FirstOrDefaultAsync(platform => platform.Id == id);
    }

    public Task<List<Platform>> GetAllAsync()
    {
        return _dbContext.Platforms.ToListAsync();
    }

    public Task<List<Platform>> GetByGameKeyAsync(string key)
    {
        return _dbContext.GamePlatforms
            .Where(link => link.Game.Key == key)
            .Select(link => link.Platform)
            .ToListAsync();
    }

    public Task<List<Platform>> GetByIdsAsync(IReadOnlyCollection<Guid> ids)
    {
        return _dbContext.Platforms
            .Where(platform => ids.Contains(platform.Id))
            .ToListAsync();
    }

    public Task AddAsync(Platform platform)
    {
        _dbContext.Platforms.Add(platform);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Platform platform)
    {
        _dbContext.Platforms.Remove(platform);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }
}
