using GameStore.Models;

namespace GameStore.Services;

public interface ICommentService
{
    Task<ServiceResult> AddCommentAsync(string gameKey,  CommentRequestDto request);

    Task<ServiceResult<List<CommentResponseDto>>> GetCommentsByGameKeyAsync(string gameKey);

    Task<ServiceResult> DeleteCommentAsync(string gameKey, Guid commentId);

    IEnumerable<string> GetBanDurations();

    Task<ServiceResult> BanUserAsync(BanRequestDto dto);

    Task<List<Comment>> GetAllDescendantsAsync(Guid gameId, Guid rootId);
}
