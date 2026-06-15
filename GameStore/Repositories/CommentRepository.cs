// CommentRepository.cs
using GameStore.Data;
using GameStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Repositories;

public class CommentRepository(GameStoreDbContext dbContext) : ICommentRepository
{
    private readonly GameStoreDbContext _dbContext = dbContext;

    public async Task<List<Comment>> GetAllByGameIdAsync(Guid gameId)
    {
        return await _dbContext.Comments
            .Where(c => c.GameId == gameId)
            .ToListAsync();
    }

    public async Task<Comment?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Comments.FindAsync(id);
    }

    public async Task AddAsync(Comment comment)
    {
        await _dbContext.Comments.AddAsync(comment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Comment comment)
    {
        _dbContext.Comments.Update(comment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Ban?> GetActiveBanAsync(string userName)
    {
        var now = DateTime.UtcNow;
        return await _dbContext.Bans.FirstOrDefaultAsync(b =>
            b.UserName == userName &&
            (b.IsPermanent || b.BannedUntil > now));
    }

    public async Task AddBanAsync(Ban ban)
    {
        await _dbContext.Bans.AddAsync(ban);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Comment>?> GetChildrenAsync(Guid parentId)
    {
        return await _dbContext.Comments
            .Where(c => c.ParentCommentId == parentId)
            .ToListAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<Comment> comments)
    {
        _dbContext.Comments.UpdateRange(comments);
        await _dbContext.SaveChangesAsync();
    }
}