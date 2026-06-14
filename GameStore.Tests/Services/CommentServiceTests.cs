using GameStore.Data;
using GameStore.Models;
using GameStore.Repositories;
using GameStore.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace GameStore.Tests.Services;

public class CommentServiceTests
{
    [Fact]
    public async Task AddCommentAsync_ReturnsBadRequest_WhenUserIsBanned()
    {
        var gameKey = "cod-mw";
        var game = new Game { Name = "Call of Duty", Id = Guid.NewGuid(), Key = gameKey };
        var dto = new CommentRequestDto
        {
            Comment = new CommentBodyDto { Name = "banned-user", Body = "nice" },
        };

        var gamesRepo = new Mock<IGameRepository>();
        gamesRepo.Setup(r => r.GetByKeyAsync(gameKey)).ReturnsAsync(game);

        var commentsRepo = new Mock<ICommentRepository>();
        commentsRepo.Setup(r => r.GetActiveBanAsync("banned-user"))
            .ReturnsAsync(new Ban { UserName = "banned-user", IsPermanent = true });

        var service = CreateService(gamesRepo, commentsRepo);
        var result = await service.AddCommentAsync(gameKey, dto);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("User is banned and cannot post comments.", result.Error);
    }

    [Fact]
    public async Task AddCommentAsync_ReturnsBadRequest_WhenParentNotFound()
    {
        var gameKey = "cod-mw";
        var game = new Game { Name = "Call of Duty", Id = Guid.NewGuid(), Key = gameKey };
        var parentId = Guid.NewGuid();
        var dto = new CommentRequestDto
        {
            Comment = new CommentBodyDto { Name = "user", Body = "reply" },
            ParentId = parentId,
            Action = "reply",
        };

        var gamesRepo = new Mock<IGameRepository>();
        gamesRepo.Setup(r => r.GetByKeyAsync(gameKey)).ReturnsAsync(game);

        var commentsRepo = new Mock<ICommentRepository>();
        commentsRepo.Setup(r => r.GetActiveBanAsync("user")).ReturnsAsync((Ban?)null);
        commentsRepo.Setup(r => r.GetByIdAsync(parentId)).ReturnsAsync((Comment?)null);

        var service = CreateService(gamesRepo, commentsRepo);
        var result = await service.AddCommentAsync(gameKey, dto);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("Parent comment not found.", result.Error);
    }

    [Fact]
    public async Task AddCommentAsync_ReturnsOk_WhenValidRootComment()
    {
        var gameKey = "cod-mw";
        var game = new Game { Name = "Call of Duty", Id = Guid.NewGuid(), Key = gameKey };
        var dto = new CommentRequestDto
        {
            Comment = new CommentBodyDto { Name = "user", Body = "nice game" },
        };

        var gamesRepo = new Mock<IGameRepository>();
        gamesRepo.Setup(r => r.GetByKeyAsync(gameKey)).ReturnsAsync(game);

        var commentsRepo = new Mock<ICommentRepository>();
        commentsRepo.Setup(r => r.GetActiveBanAsync("user")).ReturnsAsync((Ban?)null);

        var service = CreateService(gamesRepo, commentsRepo);
        var result = await service.AddCommentAsync(gameKey, dto);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        commentsRepo.Verify(
            r => r.AddAsync(It.Is<Comment>(c =>
            c.Name == "user" &&
            c.Body == "nice game" &&
            c.GameId == game.Id)),
            Times.Once);
    }

    [Fact]
    public async Task AddCommentAsync_FormatsBody_WhenActionIsReply()
    {
        var gameKey = "cod-mw";
        var game = new Game { Name = "Call of Duty", Id = Guid.NewGuid(), Key = gameKey };
        var parentId = Guid.NewGuid();
        var parent = new Comment { Id = parentId, Name = "original-author", Body = "original body", GameId = game.Id };

        var dto = new CommentRequestDto
        {
            Comment = new CommentBodyDto { Name = "user", Body = "my reply" },
            ParentId = parentId,
            Action = "reply",
        };

        var gamesRepo = new Mock<IGameRepository>();
        gamesRepo.Setup(r => r.GetByKeyAsync(gameKey)).ReturnsAsync(game);

        var commentsRepo = new Mock<ICommentRepository>();
        commentsRepo.Setup(r => r.GetActiveBanAsync("user")).ReturnsAsync((Ban?)null);
        commentsRepo.Setup(r => r.GetByIdAsync(parentId)).ReturnsAsync(parent);

        var service = CreateService(gamesRepo, commentsRepo);
        var result = await service.AddCommentAsync(gameKey, dto);

        Assert.True(result.IsSuccess);
        commentsRepo.Verify(
            r => r.AddAsync(It.Is<Comment>(c =>
            c.Body == "[original-author], my reply")),
            Times.Once);
    }

    [Fact]
    public async Task AddCommentAsync_FormatsBody_WhenActionIsQuote()
    {
        var gameKey = "cod-mw";
        var game = new Game { Name = "Call of Duty", Id = Guid.NewGuid(), Key = gameKey };
        var parentId = Guid.NewGuid();
        var parent = new Comment { Id = parentId, Name = "original-author", Body = "original body", GameId = game.Id };

        var dto = new CommentRequestDto
        {
            Comment = new CommentBodyDto { Name = "user", Body = "my quote reply" },
            ParentId = parentId,
            Action = "quote",
        };

        var gamesRepo = new Mock<IGameRepository>();
        gamesRepo.Setup(r => r.GetByKeyAsync(gameKey)).ReturnsAsync(game);

        var commentsRepo = new Mock<ICommentRepository>();
        commentsRepo.Setup(r => r.GetActiveBanAsync("user")).ReturnsAsync((Ban?)null);
        commentsRepo.Setup(r => r.GetByIdAsync(parentId)).ReturnsAsync(parent);

        var service = CreateService(gamesRepo, commentsRepo);
        var result = await service.AddCommentAsync(gameKey, dto);

        Assert.True(result.IsSuccess);
        commentsRepo.Verify(
            r => r.AddAsync(It.Is<Comment>(c =>
            c.Body == "[original body], my quote reply")),
            Times.Once);
    }

    [Fact]
    public async Task GetCommentsByGameKeyAsync_ReturnsNotFound_WhenGameMissing()
    {
        var gamesRepo = new Mock<IGameRepository>();
        gamesRepo.Setup(r => r.GetByKeyAsync("missing-key")).ReturnsAsync((Game?)null);

        var service = CreateService(gamesRepo);
        var result = await service.GetCommentsByGameKeyAsync("missing-key");

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task GetCommentsByGameKeyAsync_ReturnsNestedTree()
    {
        var gameKey = "cod-mw";
        var gameId = Guid.NewGuid();
        var game = new Game { Name = "Call of Duty", Id = gameId, Key = gameKey };

        var rootId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var comments = new List<Comment>
        {
            new() { Id = rootId,  Name = "user1", Body = "root",  ParentCommentId = null,   GameId = gameId },
            new() { Id = childId, Name = "user2", Body = "child", ParentCommentId = rootId, GameId = gameId },
        };

        var gamesRepo = new Mock<IGameRepository>();
        gamesRepo.Setup(r => r.GetByKeyAsync(gameKey)).ReturnsAsync(game);

        var commentsRepo = new Mock<ICommentRepository>();
        commentsRepo.Setup(r => r.GetAllByGameIdAsync(gameId)).ReturnsAsync(comments);

        var service = CreateService(gamesRepo, commentsRepo);
        var result = await service.GetCommentsByGameKeyAsync(gameKey);

        Assert.True(result.IsSuccess);
        var tree = result.Value!;
        Assert.Single(tree);
        Assert.Single(tree[0].ChildComments);
        Assert.Equal("child", tree[0].ChildComments[0].Body);
    }

    [Fact]
    public async Task DeleteCommentAsync_ReturnsNotFound_WhenGameMissing()
    {
        var gamesRepo = new Mock<IGameRepository>();
        gamesRepo.Setup(r => r.GetByKeyAsync("missing-key")).ReturnsAsync((Game?)null);

        var service = CreateService(gamesRepo);
        var result = await service.DeleteCommentAsync("missing-key", Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task DeleteCommentAsync_ReturnsNotFound_WhenCommentMissing()
    {
        var gameKey = "cod-mw";
        var game = new Game { Name = "Call of Duty", Id = Guid.NewGuid(), Key = gameKey };
        var commentId = Guid.NewGuid();

        var gamesRepo = new Mock<IGameRepository>();
        gamesRepo.Setup(r => r.GetByKeyAsync(gameKey)).ReturnsAsync(game);

        var commentsRepo = new Mock<ICommentRepository>();
        commentsRepo.Setup(r => r.GetByIdAsync(commentId)).ReturnsAsync((Comment?)null);

        var service = CreateService(gamesRepo, commentsRepo);
        var result = await service.DeleteCommentAsync(gameKey, commentId);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task DeleteCommentAsync_ReturnsBadRequest_WhenCommentBelongsToDifferentGame()
    {
        var gameKey = "cod-mw";
        var game = new Game { Name = "Call of Duty", Id = Guid.NewGuid(), Key = gameKey };
        var commentId = Guid.NewGuid();
        var comment = new Comment { Id = commentId, Name = "u1", Body = "Nice", GameId = Guid.NewGuid() };

        var gamesRepo = new Mock<IGameRepository>();
        gamesRepo.Setup(r => r.GetByKeyAsync(gameKey)).ReturnsAsync(game);

        var commentsRepo = new Mock<ICommentRepository>();
        commentsRepo.Setup(r => r.GetByIdAsync(commentId)).ReturnsAsync(comment);

        var service = CreateService(gamesRepo, commentsRepo);
        var result = await service.DeleteCommentAsync(gameKey, commentId);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task DeleteCommentAsync_ReplacesBodyWithDeletedMessage()
    {
        var gameKey = "cod-mw";
        var gameId = Guid.NewGuid();
        var game = new Game { Name = "Call of Duty", Id = gameId, Key = gameKey };
        var commentId = Guid.NewGuid();
        var comment = new Comment { Id = commentId, Name = "user", Body = "original", GameId = gameId };

        var gamesRepo = new Mock<IGameRepository>();
        gamesRepo.Setup(r => r.GetByKeyAsync(gameKey)).ReturnsAsync(game);

        var commentsRepo = new Mock<ICommentRepository>();
        commentsRepo.Setup(r => r.GetByIdAsync(commentId)).ReturnsAsync(comment);
        commentsRepo.Setup(r => r.GetChildrenAsync(commentId)).ReturnsAsync([]);

        var service = CreateService(gamesRepo, commentsRepo);
        var result = await service.DeleteCommentAsync(gameKey, commentId);

        Assert.True(result.IsSuccess);
        commentsRepo.Verify(
            r => r.UpdateAsync(It.Is<Comment>(c =>
            c.Id == commentId &&
            c.Body == "A comment/quote was deleted")),
            Times.Once);
    }

    [Fact]
    public async Task DeleteCommentAsync_CascadesDeletedMessageToChildren()
    {
        var gameKey = "cod-mw";
        var gameId = Guid.NewGuid();
        var game = new Game { Name = "Call of Duty", Id = gameId, Key = gameKey };
        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var parent = new Comment { Id = parentId, Name = "u1", Body = "parent body", GameId = gameId };
        var child = new Comment { Id = childId, Name = "u2", Body = "child body", GameId = gameId, ParentCommentId = parentId };

        var gamesRepo = new Mock<IGameRepository>();
        gamesRepo.Setup(r => r.GetByKeyAsync(gameKey)).ReturnsAsync(game);

        var commentsRepo = new Mock<ICommentRepository>();
        commentsRepo.Setup(r => r.GetByIdAsync(parentId)).ReturnsAsync(parent);
        commentsRepo.Setup(r => r.GetChildrenAsync(parentId)).ReturnsAsync([child]);
        commentsRepo.Setup(r => r.GetChildrenAsync(childId)).ReturnsAsync([]);

        var service = CreateService(gamesRepo, commentsRepo);
        await service.DeleteCommentAsync(gameKey, parentId);

        commentsRepo.Verify(
            r => r.UpdateAsync(It.Is<Comment>(c =>
            c.Id == childId &&
            c.Body == "A comment/quote was deleted")),
            Times.Once);
    }

    [Fact]
    public async Task BanUserAsync_ReturnsBadRequest_WhenDurationInvalid()
    {
        var dto = new BanRequestDto { User = "user", Duration = "3 years" };

        var service = CreateService();
        var result = await service.BanUserAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Theory]
    [InlineData("1 hour")]
    [InlineData("1 day")]
    [InlineData("1 week")]
    [InlineData("1 month")]
    [InlineData("permanent")]
    public async Task BanUserAsync_ReturnsOk_ForAllValidDurations(string duration)
    {
        var dto = new BanRequestDto { User = "user", Duration = duration };

        var commentsRepo = new Mock<ICommentRepository>();
        var service = CreateService(commentsRepo: commentsRepo);
        var result = await service.BanUserAsync(dto);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        commentsRepo.Verify(r => r.AddBanAsync(It.Is<Ban>(b => b.UserName == "user")), Times.Once);
    }

    [Fact]
    public async Task BanUserAsync_SetsPermanent_WhenDurationIsPermanent()
    {
        var dto = new BanRequestDto { User = "user", Duration = "permanent" };

        var commentsRepo = new Mock<ICommentRepository>();
        var service = CreateService(commentsRepo: commentsRepo);
        await service.BanUserAsync(dto);

        commentsRepo.Verify(
            r => r.AddBanAsync(It.Is<Ban>(b =>
            b.IsPermanent == true &&
            b.BannedUntil == DateTime.MaxValue)),
            Times.Once);
    }

    [Fact]
    public void GetBanDurations_ReturnsAllFiveOptions()
    {
        var service = CreateService();
        var durations = service.GetBanDurations().ToList();

        Assert.Equal(5, durations.Count);
        Assert.Contains("1 hour", durations);
        Assert.Contains("1 day", durations);
        Assert.Contains("1 week", durations);
        Assert.Contains("1 month", durations);
        Assert.Contains("permanent", durations);
    }

    private static Mock<IUnitOfWork> CreateUnitOfWork(
        Mock<IGameRepository>? gamesRepo = null,
        Mock<ICommentRepository>? commentsRepo = null)
    {
        var games = gamesRepo ?? new Mock<IGameRepository>();
        var comments = commentsRepo ?? new Mock<ICommentRepository>();

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(u => u.Games).Returns(games.Object);
        unitOfWork.SetupGet(u => u.Comments).Returns(comments.Object);
        return unitOfWork;
    }

    private static CommentService CreateService(
        Mock<IGameRepository>? gamesRepo = null,
        Mock<ICommentRepository>? commentsRepo = null)
    {
        var unitOfWork = CreateUnitOfWork(gamesRepo, commentsRepo);
        return new CommentService(unitOfWork.Object);
    }
}