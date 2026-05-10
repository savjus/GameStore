using GameStore.Models;

namespace GameStore.Repositories;

public interface IGameRepository
{
    Task<Game?> GetByKeyAsync(string key);

    Task<Game?> GetByIdAsync(Guid id);

    Task<List<Game>> GetAllAsync();

    Task<List<Game>> GetByPlatformIdAsync(Guid platformId);

    Task<List<Game>> GetByGenreIdAsync(Guid genreId);

    Task AddAsync(Game game);

    Task DeleteAsync(Game game);

    Task SaveChangesAsync();

    Task<bool> KeyExistsAsync(string gameKey, Guid excludeGameId);

    Task<Game?> GetByIdWithLinksAsync(Guid id);

    Task<bool> KeyExistsAsync(string gameKey);
}