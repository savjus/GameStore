using GameStore.Repositories;

namespace GameStore.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly GameStoreDbContext _context;
    private IGameRepository? _gameRepository;
    private IPublisherRepository? _publisherRepository;
    private IGenreRepository? _genreRepository;
    private IPlatformRepository? _platformRepository;
    private bool _disposed;

    public UnitOfWork(GameStoreDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    ~UnitOfWork()
    {
        Dispose(false);
    }

    public IGameRepository Games => _gameRepository ??= new GameRepository(_context);

    public IPublisherRepository Publishers => _publisherRepository ??= new PublisherRepository(_context);

    public IGenreRepository Genres => _genreRepository ??= new GenreRepository(_context);

    public IPlatformRepository Platforms => _platformRepository ??= new PlatformRepository(_context);

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public Task<bool> HasChangesAsync()
    {
        return Task.FromResult(_context.ChangeTracker.HasChanges());
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _context?.Dispose();
        }

        _disposed = true;
    }
}
