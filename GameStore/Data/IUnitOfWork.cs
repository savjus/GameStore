using GameStore.Repositories;

namespace GameStore.Data;

public interface IUnitOfWork : IDisposable
{
    IGameRepository Games { get; }

    IPublisherRepository Publishers { get; }

    IGenreRepository Genres { get; }

    IPlatformRepository Platforms { get; }

    IOrderRepository Orders { get; }

    ICommentRepository Comments { get; }

    Task<int> SaveChangesAsync();

    Task<bool> HasChangesAsync();
}
