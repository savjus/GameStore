using GameStore.Models;

namespace GameStore.Repositories;

public interface IGenreRepository
{
    Task<bool> ExistsAsync(Guid id);

    Task<bool> NameExistsAsync(string name, Guid? excludeId = null);

    Task<List<Genre>> GetByIdsAsync(IReadOnlyCollection<Guid> ids);

    Task<Genre?> GetByIdAsync(Guid id);

    Task<Genre?> GetByIdTrackedAsync(Guid id);

    Task<List<Genre>> GetAllAsync();

    Task<List<Genre>> GetByGameKeyAsync(string key);

    Task<List<Genre>> GetByParentIdAsync(Guid platformId);

    Task AddAsync(Genre genre);

    Task DeleteAsync(Genre genre);
}
