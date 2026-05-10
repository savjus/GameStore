using GameStore.Data;
using GameStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Repositories;

public class GameRepository(GameStoreDbContext dbContext) : IGameRepository
{
    private readonly GameStoreDbContext _dbContext = dbContext;

    public Task AddAsync(Game game)
    {
        _dbContext.Games.Add(game);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Game game)
    {
        _dbContext.Games.Remove(game);
        return Task.CompletedTask;
    }

    public Task<List<Game>> GetAllAsync()
    {
        return _dbContext.Games.ToListAsync();
    }

    public Task<Game?> GetByIdAsync(Guid id)
    {
        return _dbContext.Games.FirstOrDefaultAsync(g => g.Id == id);
    }

    public Task<Game?> GetByKeyAsync(string key)
    {
        return _dbContext.Games.FirstOrDefaultAsync(game => game.Key == key);
    }

    public Task<List<Game>> GetByGenreIdAsync(Guid genreId)
    {
        return _dbContext.Games.Where(game => game.GameGenres.Any(genre => genre.GenreId == genreId))
            .ToListAsync();
    }

    public Task<List<Game>> GetByPlatformIdAsync(Guid platformId)
    {
        return _dbContext.Games.Where(game => game.GamePlatforms.Any(platform => platform.PlatformId == platformId))
            .ToListAsync();
    }

    public Task SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }

    public Task<bool> KeyExistsAsync(string gameKey, Guid excludeGameId)
    {
        return _dbContext.Games.AnyAsync(game => game.Key == gameKey && game.Id != excludeGameId);
    }

    public Task<Game?> GetByIdWithLinksAsync(Guid id)
    {
        return _dbContext.Games
            .Include(game => game.GameGenres)
            .Include(game => game.GamePlatforms)
            .FirstOrDefaultAsync(game => game.Id == id);
    }

    public Task<bool> KeyExistsAsync(string gameKey)
    {
        return _dbContext.Games.AnyAsync(game => game.Key == gameKey);
    }
}