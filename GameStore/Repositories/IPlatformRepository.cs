using GameStore.Models;

namespace GameStore.Repositories;

public interface IPlatformRepository
{
    Task<bool> ExistsAsync(Guid id);

    Task<bool> TypeExistsAsync(string type, Guid? excludeId = null);

    Task<Platform?> GetByIdAsync(Guid id);

    Task<List<Platform>> GetAllAsync();

    Task<List<Platform>> GetByGameKeyAsync(string key);

    Task<List<Platform>> GetByIdsAsync(IReadOnlyCollection<Guid> ids);

    Task AddAsync(Platform platform);

    Task DeleteAsync(Platform platform);
}
