using GameStore.Repositories;

namespace GameStore.Data;

public interface IUnitOfWork : IDisposable
{
    IGameRepository Games { get; }

    IGenreRepository Genres { get; }

    IPlatformRepository Platforms { get; }

    Task<int> SaveChangesAsync();

    Task<bool> HasChangesAsync();
}
