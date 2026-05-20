using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using GameStore.Data;
using GameStore.Models;
using GameStore.Models.Dtos;
using GameStore.Repositories;
using GameStore.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace GameStore.Tests.Services;

public class GameServiceTests
{
    [Fact]
    public async Task GetGameByKeyAsync_ReturnsBadRequest_WhenKeyMissing()
    {
        var service = CreateService();

        var result = await service.GetGameByKeyAsync(" ");

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("Game key is required.", result.Error);
    }

    [Fact]
    public async Task GetGameByKeyAsync_ReturnsNotFound_WhenGameMissing()
    {
        var gameRepository = new Mock<IGameRepository>();
        gameRepository.Setup(repo => repo.GetByKeyAsync("missing"))
            .ReturnsAsync((Game?)null);

        var service = CreateService(gameRepository: gameRepository);

        var result = await service.GetGameByKeyAsync("missing");

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal("Game not found.", result.Error);
    }

    [Fact]
    public async Task GetGameByKeyAsync_ReturnsGame_WhenFound()
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Name = "Super Game",
            Key = "super-game",
            Description = "desc",
        };

        var gameRepository = new Mock<IGameRepository>();
        gameRepository.Setup(repo => repo.GetByKeyAsync(game.Key))
            .ReturnsAsync(game);

        var service = CreateService(gameRepository: gameRepository);

        var result = await service.GetGameByKeyAsync(game.Key);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(game.Id, result.Value!.Id);
        Assert.Equal(game.Name, result.Value.Name);
        Assert.Equal(game.Key, result.Value.Key);
        Assert.Equal(game.Description, result.Value.Description);
    }

    [Fact]
    public async Task AddGameAsync_GeneratesKey_WhenMissing()
    {
        var genreId = Guid.NewGuid();
        var platformId = Guid.NewGuid();

        var request = new AddGameRequest
        {
            Game = new GameCreateDto
            {
                Name = "Super Game!",
                Key = null,
                Description = "desc",
            },
            Genres = [genreId],
            Platforms = [platformId],
        };

        var gameRepository = new Mock<IGameRepository>();
        gameRepository.Setup(repo => repo.KeyExistsAsync("super-game"))
            .ReturnsAsync(false);

        Game? createdGame = null;
        gameRepository.Setup(repo => repo.AddAsync(It.IsAny<Game>()))
            .Callback<Game>(game => createdGame = game)
            .Returns(Task.CompletedTask);

        var genreRepository = new Mock<IGenreRepository>();
        genreRepository.Setup(repo => repo.GetByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>()))
            .ReturnsAsync([new Genre { Id = genreId, Name = "Action" }]);

        var platformRepository = new Mock<IPlatformRepository>();
        platformRepository.Setup(repo => repo.GetByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>()))
            .ReturnsAsync([new Platform { Id = platformId, Type = "Desktop" }]);

        var unitOfWork = CreateUnitOfWork(gameRepository, genreRepository, platformRepository);
        unitOfWork.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(0);
        var service = new GameService(unitOfWork.Object);

        var result = await service.AddGameAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal("Super Game!", result.Value!.Name);
        Assert.Equal("super-game", result.Value.Key);
        Assert.Equal("desc", result.Value.Description);
        Assert.NotNull(createdGame);
        Assert.Equal("super-game", createdGame!.Key);
    }

    [Fact]
    public async Task AddGameAsync_ReturnsConflict_WhenKeyExists()
    {
        var request = new AddGameRequest
        {
            Game = new GameCreateDto
            {
                Name = "Super Game",
                Key = "super-game",
            },
        };

        var gameRepository = new Mock<IGameRepository>();
        gameRepository.Setup(repo => repo.KeyExistsAsync("super-game"))
            .ReturnsAsync(true);

        var service = CreateService(gameRepository: gameRepository);

        var result = await service.AddGameAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);
        Assert.Equal("Game key 'super-game' already exists.", result.Error);
    }

    [Fact]
    public async Task AddGameAsync_ReturnsBadRequest_WhenGenreMissing()
    {
        var genreId = Guid.NewGuid();
        var request = new AddGameRequest
        {
            Game = new GameCreateDto
            {
                Name = "Super Game",
                Key = "super-game",
            },
            Genres = [genreId],
        };

        var gameRepository = new Mock<IGameRepository>();
        gameRepository.Setup(repo => repo.KeyExistsAsync("super-game"))
            .ReturnsAsync(false);

        var genreRepository = new Mock<IGenreRepository>();
        genreRepository.Setup(repo => repo.GetByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>()))
            .ReturnsAsync([]);

        var service = CreateService(gameRepository, genreRepository);

        var result = await service.AddGameAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("One or more genres do not exist.", result.Error);
    }

    [Fact]
    public async Task UpdateGameAsync_ReturnsNotFound_WhenGameMissing()
    {
        var request = new UpdateGameRequest
        {
            Game = new GameUpdateDto
            {
                Id = Guid.NewGuid(),
                Name = "Super Game",
                Key = "super-game",
            },
        };

        var gameRepository = new Mock<IGameRepository>();
        gameRepository.Setup(repo => repo.GetByIdWithLinksAsync(request.Game.Id))
            .ReturnsAsync((Game?)null);

        var service = CreateService(gameRepository: gameRepository);

        var result = await service.UpdateGameAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal("Game not found.", result.Error);
    }

    [Fact]
    public async Task UpdateGameAsync_ReturnsUpdatedGame_WhenValid()
    {
        var gameId = Guid.NewGuid();
        var genreId = Guid.NewGuid();
        var platformId = Guid.NewGuid();
        var game = new Game
        {
            Id = gameId,
            Name = "Old",
            Key = "old",
            Description = "old",
            GameGenres = [],
            GamePlatforms = [],
        };

        var request = new UpdateGameRequest
        {
            Game = new GameUpdateDto
            {
                Id = gameId,
                Name = "New Name",
                Key = "new-key",
                Description = "new",
            },
            Genres = [genreId],
            Platforms = [platformId],
        };

        var gameRepository = new Mock<IGameRepository>();
        gameRepository.Setup(repo => repo.GetByIdWithLinksAsync(gameId))
            .ReturnsAsync(game);
        gameRepository.Setup(repo => repo.KeyExistsAsync("new-key", gameId))
            .ReturnsAsync(false);

        var genreRepository = new Mock<IGenreRepository>();
        genreRepository.Setup(repo => repo.GetByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>()))
            .ReturnsAsync([new Genre { Id = genreId, Name = "Action" }]);

        var platformRepository = new Mock<IPlatformRepository>();
        platformRepository.Setup(repo => repo.GetByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>()))
            .ReturnsAsync([new Platform { Id = platformId, Type = "Desktop" }]);

        var unitOfWork = CreateUnitOfWork(gameRepository, genreRepository, platformRepository);
        unitOfWork.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(0);
        var service = new GameService(unitOfWork.Object);

        var result = await service.UpdateGameAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal("New Name", result.Value!.Name);
        Assert.Equal("new-key", result.Value.Key);
        Assert.Equal("new", result.Value.Description);
    }

    [Fact]
    public async Task DeleteGameAsync_ReturnsNotFound_WhenGameMissing()
    {
        var gameRepository = new Mock<IGameRepository>();
        gameRepository.Setup(repo => repo.GetByKeyAsync("missing"))
            .ReturnsAsync((Game?)null);

        var service = CreateService(gameRepository: gameRepository);

        var result = await service.DeleteGameAsync("missing");

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal("Game not found.", result.Error);
    }

    [Fact]
    public async Task GetGameFileAsync_ReturnsFile_WithExpectedName()
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Name = "Super Game",
            Key = "super-game",
        };

        var gameRepository = new Mock<IGameRepository>();
        gameRepository.Setup(repo => repo.GetByKeyAsync(game.Key))
            .ReturnsAsync(game);

        var service = CreateService(gameRepository: gameRepository);

        var result = await service.GetGameFileAsync(game.Key);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal("text/plain", result.Value!.ContentType);
        Assert.Matches(new Regex("^Super Game_\\d{14}\\.txt$"), result.Value.FileName);

        var json = Encoding.UTF8.GetString(result.Value.Content);
        using var document = JsonDocument.Parse(json);
        Assert.Equal("Super Game", document.RootElement.GetProperty("Name").GetString());
        Assert.Equal("super-game", document.RootElement.GetProperty("Key").GetString());
    }

    private static Mock<IUnitOfWork> CreateUnitOfWork(
        Mock<IGameRepository>? gameRepository = null,
        Mock<IGenreRepository>? genreRepository = null,
        Mock<IPlatformRepository>? platformRepository = null)
    {
        var gameRepo = gameRepository ?? new Mock<IGameRepository>();
        var genreRepo = genreRepository ?? new Mock<IGenreRepository>();
        var platformRepo = platformRepository ?? new Mock<IPlatformRepository>();

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(u => u.Games).Returns(gameRepo.Object);
        unitOfWork.SetupGet(u => u.Genres).Returns(genreRepo.Object);
        unitOfWork.SetupGet(u => u.Platforms).Returns(platformRepo.Object);
        return unitOfWork;
    }

    private static GameService CreateService(
        Mock<IGameRepository>? gameRepository = null,
        Mock<IGenreRepository>? genreRepository = null,
        Mock<IPlatformRepository>? platformRepository = null)
    {
        var unitOfWork = CreateUnitOfWork(gameRepository, genreRepository, platformRepository);
        return new GameService(unitOfWork.Object);
    }
}
