using GameStore.Data;
using GameStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Repositories;

public class CommentRepository(GameStoreDbContext dbContext) : ICommentRepository
{
    private readonly GameStoreDbContext _dbContext = dbContext;

    public Task<List<Comment>> GetAllByGameIdAsync(Guid gameId)
    {
        return _dbContext.Comments
            .Where(c => c.GameId == gameId)
            .OrderBy(c => c.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<Comment?> GetByIdAsync(Guid id)
    {
        return _dbContext.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    }

    public Task AddAsync(Comment comment)
    {
        _dbContext.Comments.Add(comment);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Comment comment)
    {
        _dbContext.Comments.Update(comment);
        return Task.CompletedTask;
    }

    public Task<Ban?> GetActiveBanAsync(string userName)
    {
        var now = DateTime.UtcNow;
        return _dbContext.Bans.FirstOrDefaultAsync(b =>
            b.UserName == userName &&
            (b.IsPermanent || b.BannedUntil > now));
    }

    public Task AddBanAsync(Ban ban)
    {
        _dbContext.Bans.Add(ban);
        return Task.CompletedTask;
    }

    public Task<List<Comment>> GetChildrenAsync(Guid parentId)
    {
        return _dbContext.Comments
            .Where(c => c.ParentCommentId == parentId)
            .ToListAsync();
    }

    public Task UpdateRange(IEnumerable<Comment> comments)
    {
        _dbContext.Comments.UpdateRange(comments);
        return Task.CompletedTask;
    }
}