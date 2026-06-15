using GameStore.Data;
using GameStore.Models;

namespace GameStore.Services;

public class CommentService(IUnitOfWork unitOfWork) : ICommentService
{
    private const string DeletedMessage = "A comment/quote was deleted";
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    private static readonly Dictionary<string, TimeSpan?> BanDurations = new()
    {
        ["1 hour"] = TimeSpan.FromHours(1),
        ["1 day"] = TimeSpan.FromDays(1),
        ["1 week"] = TimeSpan.FromDays(7),
        ["1 month"] = TimeSpan.FromDays(30),
        ["permanent"] = null,
    };

    public IEnumerable<string> GetBanDurations() => BanDurations.Keys;

    public async Task<ServiceResult> AddCommentAsync(string gameKey, CommentRequestDto dto)
    {
        var game = await _unitOfWork.Games.GetByKeyAsync(gameKey);
        if (game is null)
        {
            return ServiceResult.Fail(StatusCodes.Status404NotFound, $"Game '{gameKey}' not found.");
        }

        var activeBan = await _unitOfWork.Comments.GetActiveBanAsync(dto.Comment.Name);
        if (activeBan is not null)
        {
            return ServiceResult.Fail(
                StatusCodes.Status400BadRequest,
                "User is banned and cannot post comments.");
        }

        string body = dto.Comment.Body;

        if (dto.ParentId.HasValue)
        {
            var parent = await _unitOfWork.Comments.GetByIdAsync(dto.ParentId.Value);
            if (parent is null)
            {
                return ServiceResult.Fail(
                    StatusCodes.Status400BadRequest,
                    "Parent comment not found.");
            }

            var action = dto.Action?.Trim().ToLower();

            body = action switch
            {
                "reply" => BuildReplyBody(parent, dto.Comment.Body),
                "quote" => BuildQuoteBody(parent, dto.Comment.Body),
                _ => dto.Comment.Body,
            };
        }

        var comment = new Comment
        {
            Name = dto.Comment.Name,
            Body = body,
            ParentCommentId = dto.ParentId,
            GameId = game.Id,
        };

        await _unitOfWork.Comments.AddAsync(comment);
        return ServiceResult.Success(StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<List<CommentResponseDto>>> GetCommentsByGameKeyAsync(string gameKey)
    {
        var game = await _unitOfWork.Games.GetByKeyAsync(gameKey);
        if (game is null)
        {
            return ServiceResult.Fail<List<CommentResponseDto>>(StatusCodes.Status404NotFound, $"Game '{gameKey}' not found.");
        }

        var all = await _unitOfWork.Comments.GetAllByGameIdAsync(game.Id);

        return ServiceResult.Success(BuildTree(all, null), StatusCodes.Status200OK);
    }

    public async Task<ServiceResult> DeleteCommentAsync(string gameKey, Guid commentId)
    {
        var game = await _unitOfWork.Games.GetByKeyAsync(gameKey);
        if (game is null)
        {
            return ServiceResult.Fail(StatusCodes.Status404NotFound, $"Game '{gameKey}' not found.");
        }

        var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
        if (comment is null)
        {
            return ServiceResult.Fail(StatusCodes.Status404NotFound, "Comment not found.");
        }

        if (comment.GameId != game.Id)
        {
            return ServiceResult.Fail(StatusCodes.Status400BadRequest, "Comment does not belong to this game.");
        }

        comment.Body = DeletedMessage;
        await _unitOfWork.Comments.UpdateAsync(comment);
        await CascadeDeleteMessageAsync(comment.Id);

        return ServiceResult.Success(StatusCodes.Status200OK);
    }

    public async Task<ServiceResult> BanUserAsync(BanRequestDto dto)
    {
        if (!BanDurations.TryGetValue(dto.Duration.Trim().ToLower(), out var span))
        {
            return ServiceResult.Fail(StatusCodes.Status400BadRequest, $"Invalid ban duration: '{dto.Duration}'.");
        }

        var ban = new Ban
        {
            UserName = dto.User,
            IsPermanent = span is null,
            BannedUntil = span.HasValue ? DateTime.UtcNow.Add(span.Value) : DateTime.MaxValue,
        };

        await _unitOfWork.Comments.AddBanAsync(ban);
        return ServiceResult.Success(StatusCodes.Status200OK);
    }

    private static string BuildReplyBody(Comment parent, string replyText)
    {
        var authorName = parent.Name;
        return $"[{authorName}], {replyText}";
    }

    private static string BuildQuoteBody(Comment parent, string quoteText)
    {
        return $"\"{parent.Body}\", {quoteText}";
    }

    private List<CommentResponseDto> BuildTree(List<Comment> all, Guid? parentId)
    {
        return all
            .Where(c => c.ParentCommentId == parentId)
            .Select(c => new CommentResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Body = c.Body,
                ChildComments = BuildTree(all, c.Id),
            })
            .ToList();
    }

    private async Task CascadeDeleteMessageAsync(Guid parentId)
    {
        var children = await _unitOfWork.Comments.GetChildrenAsync(parentId);

        foreach (var child in children)
        {
            child.Body = DeletedMessage;
            await _unitOfWork.Comments.UpdateAsync(child);
            await CascadeDeleteMessageAsync(child.Id);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}