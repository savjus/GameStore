using GameStore.Data;
using GameStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Repositories;

public class GenreRepository(GameStoreDbContext dbContext) : IGenreRepository
{
    private readonly GameStoreDbContext _dbContext = dbContext;

    public Task AddAsync(Genre genre)
    {
        _dbContext.Genres.Add(genre);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Genre genre)
    {
        _dbContext.Genres.Remove(genre);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        return _dbContext.Genres.AnyAsync(genre => genre.Id == id);
    }

    public Task<List<Genre>> GetAllAsync()
    {
        return _dbContext.Genres.AsNoTracking().ToListAsync();
    }

    public Task<List<Genre>> GetByGameKeyAsync(string key)
    {
        return _dbContext.GameGenres
            .AsNoTracking()
            .Where(link => link.Game.Key == key)
            .Select(link => link.Genre)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<Genre?> GetByIdAsync(Guid id)
    {
        return _dbContext.Genres.AsNoTracking().FirstOrDefaultAsync(genre => genre.Id == id);
    }

    public Task<List<Genre>> GetByIdsAsync(IReadOnlyCollection<Guid> ids)
    {
        return _dbContext.Genres
            .AsNoTracking()
            .Where(genre => ids.Contains(genre.Id))
            .ToListAsync();
    }

    public Task<List<Genre>> GetByParentIdAsync(Guid platformId)
    {
        return _dbContext.Genres
            .AsNoTracking()
            .Where(genre => genre.ParentGenreId == platformId)
            .ToListAsync();
    }

    public Task SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }
}
