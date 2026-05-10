using GameStore.Repositories;

namespace GameStore.Data;

public class UnitOfWork(GameStoreDbContext context) : IUnitOfWork
{
    private readonly GameStoreDbContext _context = context;
    private IGameRepository? _gameRepository;
    private IGenreRepository? _genreRepository;
    private IPlatformRepository? _platformRepository;

    public IGameRepository Games => _gameRepository ??= new GameRepository(_context);

    public IGenreRepository Genres => _genreRepository ??= new GenreRepository(_context);

    public IPlatformRepository Platforms => _platformRepository ??= new PlatformRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<bool> HasChangesAsync()
    {
        return await Task.FromResult(_context.ChangeTracker.HasChanges());
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
