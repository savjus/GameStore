using GameStore.Models;

namespace GameStore.Repositories;

public interface ICommentRepository
{
    Task<List<Comment>> GetAllByGameIdAsync(Guid gameId);

    Task<Comment?> GetByIdAsync(Guid id);

    Task<Ban?> GetActiveBanAsync(string userName);

    Task AddAsync(Comment comment);

    Task UpdateAsync(Comment comment);

    Task AddBanAsync(Ban ban);

    Task<List<Comment>?> GetChildrenAsync(Guid parentId);

    Task UpdateRangeAsync(IEnumerable<Comment> comments);
}