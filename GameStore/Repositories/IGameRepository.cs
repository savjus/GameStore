using GameStore.Models;

namespace GameStore.Repositories;

public interface IGameRepository
{
    Task<Game?> GetByKeyAsync(string key);

    Task<Game?> GetByIdAsync(Guid id);

    Task<List<Game>> GetAllAsync();

    Task<List<Game>> GetByPlatformIdAsync(Guid id);

    Task<List<Game>> GetByGenreIdAsync(Guid id);

    Task AddAsync(Game game);

    Task DeleteAsync(Game game);

    Task SaveChangesAsync();
}