using GameStore.Models;

namespace GameStore.Repositories;

public interface IGenreRepository
{
    Task<bool> ExistsAsync(Guid id);

    Task<List<Genre>> GetByIdsAsync(IReadOnlyCollection<Guid> ids);

    Task<Genre?> GetByIdAsync(Guid id);

    Task<List<Genre>> GetAllAsync();

    Task<List<Genre>> GetByGameKeyAsync(string key);

    Task<List<Genre>> GetByParentIdAsync(Guid platformId);

    Task AddAsync(Genre genre);

    Task DeleteAsync(Genre genre);
}
