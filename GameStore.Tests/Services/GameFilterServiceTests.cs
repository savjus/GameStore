using GameStore.Data;
using GameStore.Models;
using GameStore.Models.Dtos;
using GameStore.Repositories;
using GameStore.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace GameStore.Tests.Services;

public class GameFilterServiceTests
{
    [Fact]
    public async Task GetPaginationOptionsAsync_ReturnsExpectedOptions()
    {
        var service = CreateService();

        var result = await service.GetPaginationOptionsAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(["10", "20", "50", "100", "all"], result.Value);
    }

    [Fact]
    public async Task GetSortingOptionsAsync_ReturnsExpectedOptions()
    {
        var service = CreateService();

        var result = await service.GetSortingOptionsAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(["Most popular", "Most commented", "Price ASC", "Price DESC", "New"], result.Value);
    }

    [Fact]
    public async Task GetPublishDateFilterOptionsAsync_ReturnsExpectedOptions()
    {
        var service = CreateService();

        var result = await service.GetPublishDateFilterOptionsAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(["last week", "last month", "last year", "2 years", "3 years"], result.Value);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_NoFilter_ReturnsAllGames()
    {
        var games = MakeGames(3);
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { PageSize = "all", Page = 1 };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value!.Games.Count);
        Assert.Equal(1, result.Value.TotalPages);
        Assert.Equal(1, result.Value.CurrentPage);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_WithGenreFilter_ReturnsMatchingGames()
    {
        var genreId = Guid.NewGuid();
        var games = new List<Game>
        {
            MakeGame(genreIds: [genreId]),
            MakeGame(),
            MakeGame(),
        };
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { GenreIds = [genreId], PageSize = "all" };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Games);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_WithPlatformFilter_ReturnsMatchingGames()
    {
        var platformId = Guid.NewGuid();
        var games = new List<Game>
        {
            MakeGame(platformIds: [platformId]),
            MakeGame(),
        };
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { PlatformIds = [platformId], PageSize = "all" };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Games);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_WithPublisherFilter_ReturnsMatchingGames()
    {
        var publisherId = Guid.NewGuid();
        var games = new List<Game>
        {
            MakeGame(publisherId: publisherId),
            MakeGame(),
            MakeGame(),
        };
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { PublisherIds = [publisherId], PageSize = "all" };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Games);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_WithMinPrice_ExcludesCheaperGames()
    {
        var games = new List<Game>
        {
            MakeGame(price: 5m),
            MakeGame(price: 20m),
            MakeGame(price: 50m),
        };
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { MinPrice = 15m, PageSize = "all" };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Games.Count);
        Assert.All(result.Value.Games, g => Assert.True(g.Price >= 15m));
    }

    [Fact]
    public async Task GetFilteredGamesAsync_WithMaxPrice_ExcludesExpensiveGames()
    {
        var games = new List<Game>
        {
            MakeGame(price: 5m),
            MakeGame(price: 20m),
            MakeGame(price: 50m),
        };
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { MaxPrice = 25m, PageSize = "all" };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Games.Count);
        Assert.All(result.Value.Games, g => Assert.True(g.Price <= 25m));
    }

    [Fact]
    public async Task GetFilteredGamesAsync_WithPriceRange_ReturnsGamesWithinRange()
    {
        var games = new List<Game>
        {
            MakeGame(price: 5m),
            MakeGame(price: 20m),
            MakeGame(price: 50m),
        };
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { MinPrice = 10m, MaxPrice = 30m, PageSize = "all" };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Games);
        Assert.Equal(20m, result.Value.Games[0].Price);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_WithNameFilter_ReturnsMatchingGames()
    {
        var games = new List<Game>
        {
            MakeGame(name: "Super Mario"),
            MakeGame(name: "Legend of Zelda"),
            MakeGame(name: "Super Metroid"),
        };
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { Name = "Super", PageSize = "all" };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Games.Count);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_WithNameFilterLessThan3Chars_ReturnsAllGames()
    {
        var games = MakeGames(3);
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { Name = "Su", PageSize = "all" };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value!.Games.Count);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_WithLastWeekFilter_ReturnsRecentGames()
    {
        var games = new List<Game>
        {
            MakeGame(createdAt: DateTime.UtcNow.AddDays(-3)),
            MakeGame(createdAt: DateTime.UtcNow.AddDays(-10)),
        };
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { PublishDateFilter = "last week", PageSize = "all" };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Games);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_WithLastMonthFilter_ReturnsRecentGames()
    {
        var games = new List<Game>
        {
            MakeGame(createdAt: DateTime.UtcNow.AddDays(-15)),
            MakeGame(createdAt: DateTime.UtcNow.AddDays(-45)),
        };
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { PublishDateFilter = "last month", PageSize = "all" };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Games);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_SortByPriceAsc_ReturnsCheapestFirst()
    {
        var games = new List<Game>
        {
            MakeGame(price: 50m),
            MakeGame(price: 10m),
            MakeGame(price: 30m),
        };
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { SortBy = "Price ASC", PageSize = "all" };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Equal(10m, result.Value!.Games[0].Price);
        Assert.Equal(30m, result.Value.Games[1].Price);
        Assert.Equal(50m, result.Value.Games[2].Price);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_SortByPriceDesc_ReturnsMostExpensiveFirst()
    {
        var games = new List<Game>
        {
            MakeGame(price: 50m),
            MakeGame(price: 10m),
            MakeGame(price: 30m),
        };
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { SortBy = "Price DESC", PageSize = "all" };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Equal(50m, result.Value!.Games[0].Price);
        Assert.Equal(30m, result.Value.Games[1].Price);
        Assert.Equal(10m, result.Value.Games[2].Price);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_SortByNew_ReturnsNewestFirst()
    {
        var now = DateTime.UtcNow;
        var games = new List<Game>
        {
            MakeGame(createdAt: now.AddDays(-10)),
            MakeGame(createdAt: now.AddDays(-1)),
            MakeGame(createdAt: now.AddDays(-5)),
        };
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { SortBy = "New", PageSize = "all" };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Equal(now.AddDays(-1).Date, result.Value!.Games[0].CreatedAt.Date);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_SortByMostPopular_ReturnsHighestViewsFirst()
    {
        var games = new List<Game>
        {
            MakeGame(viewCount: 5),
            MakeGame(viewCount: 100),
            MakeGame(viewCount: 30),
        };
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { SortBy = "Most popular", PageSize = "all" };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Equal(100, result.Value!.Games[0].ViewCount);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_SortByMostCommented_ReturnsHighestCommentsFirst()
    {
        var games = new List<Game>
        {
            MakeGame(commentCount: 2),
            MakeGame(commentCount: 10),
            MakeGame(commentCount: 5),
        };
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { SortBy = "Most commented", PageSize = "all" };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value!.Games[0].CommentsCount);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_Pagination_ReturnsCorrectPage()
    {
        var games = MakeGames(25);
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { PageSize = "10", Page = 2 };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value!.Games.Count);
        Assert.Equal(3, result.Value.TotalPages);
        Assert.Equal(2, result.Value.CurrentPage);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_PaginationAll_ReturnsAllGames()
    {
        var games = MakeGames(55);
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { PageSize = "all", Page = 1 };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Equal(55, result.Value!.Games.Count);
        Assert.Equal(1, result.Value.TotalPages);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_MultipleGenres_TreatedAsOr()
    {
        var genreId1 = Guid.NewGuid();
        var genreId2 = Guid.NewGuid();
        var games = new List<Game>
        {
            MakeGame(genreIds: [genreId1]),
            MakeGame(genreIds: [genreId2]),
            MakeGame(genreIds: [Guid.NewGuid()]),
        };
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { GenreIds = [genreId1, genreId2], PageSize = "all" };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Games.Count);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_GenreAndPlatform_TreatedAsAnd()
    {
        var genreId = Guid.NewGuid();
        var platformId = Guid.NewGuid();
        var games = new List<Game>
        {
            MakeGame(genreIds: [genreId], platformIds: [platformId]),
            MakeGame(genreIds: [genreId]),
            MakeGame(platformIds: [platformId]),
        };
        var repo = SetupGameRepository(games);
        var service = CreateService(gameRepository: repo);

        var filter = new GameFilterRequest { GenreIds = [genreId], PlatformIds = [platformId], PageSize = "all" };
        var result = await service.GetFilteredGamesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Games);
    }

    [Fact]
    public async Task GetGameByKeyAsync_IncrementsViewCount()
    {
        var trackingGame = new Game
        {
            Id = Guid.NewGuid(),
            Name = "Test Game",
            Key = "test-game",
            ViewCount = 0,
            CreatedAt = DateTime.UtcNow,
            PublisherId = Guid.NewGuid(),
        };

        var gameRepository = new Mock<IGameRepository>();
        gameRepository.Setup(r => r.GetByKeyTrackingAsync(trackingGame.Key)).ReturnsAsync(trackingGame);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(u => u.Games).Returns(gameRepository.Object);
        unitOfWork.SetupGet(u => u.Genres).Returns(new Mock<IGenreRepository>().Object);
        unitOfWork.SetupGet(u => u.Platforms).Returns(new Mock<IPlatformRepository>().Object);
        unitOfWork.SetupGet(u => u.Publishers).Returns(new Mock<IPublisherRepository>().Object);
        unitOfWork.SetupGet(u => u.Orders).Returns(new Mock<IOrderRepository>().Object);
        unitOfWork.SetupGet(u => u.Comments).Returns(new Mock<ICommentRepository>().Object);
        unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var service = new GameService(unitOfWork.Object);
        await service.GetGameByKeyAsync(trackingGame.Key);

        unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        Assert.Equal(1, trackingGame.ViewCount);
    }

    private static List<Game> MakeGames(int count)
    {
        return Enumerable.Range(0, count).Select(_ => MakeGame()).ToList();
    }

    private static Game MakeGame(
        string? name = null,
        decimal price = 10m,
        Guid? publisherId = null,
        List<Guid>? genreIds = null,
        List<Guid>? platformIds = null,
        DateTime? createdAt = null,
        int viewCount = 0,
        int commentCount = 0)
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Name = name ?? $"Game_{Guid.NewGuid():N}",
            Key = $"game-{Guid.NewGuid():N}",
            Price = price,
            PublisherId = publisherId ?? Guid.NewGuid(),
            ViewCount = viewCount,
            CreatedAt = createdAt ?? DateTime.UtcNow,
        };

        if (genreIds != null)
        {
            foreach (var gId in genreIds)
            {
                game.GameGenres.Add(new GameGenre { GameId = game.Id, GenreId = gId });
            }
        }

        if (platformIds != null)
        {
            foreach (var pId in platformIds)
            {
                game.GamePlatforms.Add(new GamePlatform { GameId = game.Id, PlatformId = pId });
            }
        }

        for (var i = 0; i < commentCount; i++)
        {
            game.Comments.Add(new Comment
            {
                Id = Guid.NewGuid(),
                GameId = game.Id,
                Body = "comment",
                Name = "author",
            });
        }

        return game;
    }

    private static Mock<IGameRepository> SetupGameRepository(List<Game> games)
    {
        var repo = new Mock<IGameRepository>();
        repo.Setup(r => r.GetQueryable()).Returns(games.AsQueryable());
        return repo;
    }

    private static GameService CreateService(Mock<IGameRepository>? gameRepository = null)
    {
        var gameRepo = gameRepository ?? new Mock<IGameRepository>();
        var genreRepo = new Mock<IGenreRepository>();
        var platformRepo = new Mock<IPlatformRepository>();
        var publisherRepo = new Mock<IPublisherRepository>();
        var orderRepo = new Mock<IOrderRepository>();
        var commentRepo = new Mock<ICommentRepository>();

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(u => u.Games).Returns(gameRepo.Object);
        unitOfWork.SetupGet(u => u.Genres).Returns(genreRepo.Object);
        unitOfWork.SetupGet(u => u.Platforms).Returns(platformRepo.Object);
        unitOfWork.SetupGet(u => u.Publishers).Returns(publisherRepo.Object);
        unitOfWork.SetupGet(u => u.Orders).Returns(orderRepo.Object);
        unitOfWork.SetupGet(u => u.Comments).Returns(commentRepo.Object);

        return new GameService(unitOfWork.Object);
    }
}
