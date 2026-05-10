using System.Text;
using System.Text.Json;
using GameStore.Models;
using GameStore.Models.Dtos;
using GameStore.Repositories;

namespace GameStore.Services;

public class GameService(
    IGameRepository gameRepository,
    IGenreRepository genreRepository,
    IPlatformRepository platformRepository) : IGameService
{
    private readonly IGameRepository _gameRepository = gameRepository;
    private readonly IGenreRepository _genreRepository = genreRepository;
    private readonly IPlatformRepository _platformRepository = platformRepository;

    public async Task<ServiceResult<GameResponseDto>> GetGameByKeyAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status400BadRequest,
                "Game key is required.");
        }

        var game = await _gameRepository.GetByKeyAsync(key);
        return game == null
            ? ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status404NotFound,
                "Game not found.")
            : ServiceResult.Success(MapToResponse(game), StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<GameResponseDto>> GetGameByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status400BadRequest,
                "Game id is required.");
        }

        var game = await _gameRepository.GetByIdAsync(id);
        return game == null
            ? ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status404NotFound,
                "Game not found.")
            : ServiceResult.Success(MapToResponse(game), StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<List<GameResponseDto>>> GetAllGamesAsync()
    {
        var games = await _gameRepository.GetAllAsync();
        var response = games.Select(MapToResponse).ToList();
        return ServiceResult.Success(response, StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<List<GameResponseDto>>> GetGamesByPlatformAsync(Guid platformId)
    {
        if (platformId == Guid.Empty)
        {
            return ServiceResult.Fail<List<GameResponseDto>>(
                StatusCodes.Status400BadRequest,
                "Platform id is required.");
        }

        var platformExists = await _platformRepository.ExistsAsync(platformId);
        if (!platformExists)
        {
            return ServiceResult.Fail<List<GameResponseDto>>(
                StatusCodes.Status404NotFound,
                "Platform not found.");
        }

        var games = await _gameRepository.GetByPlatformIdAsync(platformId);
        var response = games.Select(MapToResponse).ToList();
        return ServiceResult.Success(response, StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<List<GameResponseDto>>> GetGamesByGenreAsync(Guid genreId)
    {
        if (genreId == Guid.Empty)
        {
            return ServiceResult.Fail<List<GameResponseDto>>(
                StatusCodes.Status400BadRequest,
                "Genre id is required.");
        }

        var genreExists = await _genreRepository.ExistsAsync(genreId);
        if (!genreExists)
        {
            return ServiceResult.Fail<List<GameResponseDto>>(
                StatusCodes.Status404NotFound,
                "Genre not found.");
        }

        var games = await _gameRepository.GetByGenreIdAsync(genreId);
        var response = games.Select(MapToResponse).ToList();
        return ServiceResult.Success(response, StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<GameResponseDto>> AddGameAsync(AddGameRequest request)
    {
        if (request.Game == null || string.IsNullOrWhiteSpace(request.Game.Name))
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status400BadRequest,
                "Game name is required.");
        }

        var gameName = request.Game.Name.Trim();
        var gameKey = string.IsNullOrWhiteSpace(request.Game.Key)
            ? GenerateKey(gameName)
            : request.Game.Key.Trim();

        var keyExists = await _gameRepository.KeyExistsAsync(gameKey);
        if (keyExists)
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status409Conflict,
                $"Game key '{gameKey}' already exists.");
        }

        var game = new Game
        {
            Id = Guid.NewGuid(),
            Name = gameName,
            Key = gameKey,
            Description = request.Game.Description?.Trim(),
        };

        if (request.Genres.Count > 0)
        {
            var distinctGenreIds = request.Genres.Distinct().ToList();
            var existingGenres = await _genreRepository.GetByIdsAsync(distinctGenreIds);

            if (existingGenres.Count != distinctGenreIds.Count)
            {
                return ServiceResult.Fail<GameResponseDto>(
                    StatusCodes.Status400BadRequest,
                    "One or more genres do not exist.");
            }

            foreach (var genre in existingGenres)
            {
                game.GameGenres.Add(new GameGenre
                {
                    GameId = game.Id,
                    GenreId = genre.Id,
                    Game = game,
                    Genre = genre,
                });
            }
        }

        if (request.Platforms.Count > 0)
        {
            var distinctPlatformIds = request.Platforms.Distinct().ToList();
            var existingPlatforms = await _platformRepository.GetByIdsAsync(distinctPlatformIds);

            if (existingPlatforms.Count != distinctPlatformIds.Count)
            {
                return ServiceResult.Fail<GameResponseDto>(
                    StatusCodes.Status400BadRequest,
                    "One or more platforms do not exist.");
            }

            foreach (var platform in existingPlatforms)
            {
                game.GamePlatforms.Add(new GamePlatform
                {
                    GameId = game.Id,
                    PlatformId = platform.Id,
                    Game = game,
                    Platform = platform,
                });
            }
        }

        await _gameRepository.AddAsync(game);
        await _gameRepository.SaveChangesAsync();

        var response = new GameResponseDto
        {
            Id = game.Id,
            Name = game.Name,
            Key = game.Key,
            Description = game.Description,
        };

        return ServiceResult.Success(response, StatusCodes.Status201Created);
    }

    public async Task<ServiceResult<GameResponseDto>> UpdateGameAsync(UpdateGameRequest request)
    {
        if (request.Game == null || request.Game.Id == Guid.Empty)
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status400BadRequest,
                "Game id is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Game.Name))
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status400BadRequest,
                "Game name is required.");
        }

        var game = await _gameRepository.GetByIdWithLinksAsync(request.Game.Id);
        if (game == null)
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status404NotFound,
                "Game not found.");
        }

        var gameName = request.Game.Name.Trim();
        var gameKey = string.IsNullOrWhiteSpace(request.Game.Key)
            ? GenerateKey(gameName)
            : request.Game.Key.Trim();

        var keyExists = await _gameRepository.KeyExistsAsync(gameKey, game.Id);
        if (keyExists)
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status409Conflict,
                $"Game key '{gameKey}' already exists.");
        }

        game.Name = gameName;
        game.Key = gameKey;
        game.Description = request.Game.Description?.Trim();

        var distinctGenreIds = request.Genres.Distinct().ToList();
        var existingGenres = distinctGenreIds.Count == 0
            ? []
            : await _genreRepository.GetByIdsAsync(distinctGenreIds);

        if (existingGenres.Count != distinctGenreIds.Count)
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status400BadRequest,
                "One or more genres do not exist.");
        }

        var distinctPlatformIds = request.Platforms.Distinct().ToList();
        var existingPlatforms = distinctPlatformIds.Count == 0
            ? []
            : await _platformRepository.GetByIdsAsync(distinctPlatformIds);

        if (existingPlatforms.Count != distinctPlatformIds.Count)
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status400BadRequest,
                "One or more platforms do not exist.");
        }

        game.GameGenres.Clear();
        foreach (var genre in existingGenres)
        {
            game.GameGenres.Add(new GameGenre
            {
                GameId = game.Id,
                GenreId = genre.Id,
                Game = game,
                Genre = genre,
            });
        }

        game.GamePlatforms.Clear();
        foreach (var platform in existingPlatforms)
        {
            game.GamePlatforms.Add(new GamePlatform
            {
                GameId = game.Id,
                PlatformId = platform.Id,
                Game = game,
                Platform = platform,
            });
        }

        await _gameRepository.SaveChangesAsync();

        return ServiceResult.Success(MapToResponse(game), StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<GameResponseDto>> DeleteGameAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status400BadRequest,
                "Game key is required.");
        }

        var game = await _gameRepository.GetByKeyAsync(key);
        if (game == null)
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status404NotFound,
                "Game not found.");
        }

        await _gameRepository.DeleteAsync(game);
        await _gameRepository.SaveChangesAsync();

        return ServiceResult.Success(MapToResponse(game), StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<GameFileDto>> GetGameFileAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return ServiceResult.Fail<GameFileDto>(
                StatusCodes.Status400BadRequest,
                "Game key is required.");
        }

        var game = await _gameRepository.GetByKeyAsync(key);
        if (game == null)
        {
            return ServiceResult.Fail<GameFileDto>(
                StatusCodes.Status404NotFound,
                "Game not found.");
        }

        var response = MapToResponse(game);
        var json = JsonSerializer.Serialize(response);
        var content = Encoding.UTF8.GetBytes(json);
        var fileName = BuildFileName(game.Name);

        return ServiceResult.Success(
            new GameFileDto
            {
                FileName = fileName,
                ContentType = "text/plain",
                Content = content,
            },
            StatusCodes.Status200OK);
    }

    private static GameResponseDto MapToResponse(Game game)
    {
        return new GameResponseDto
        {
            Id = game.Id,
            Name = game.Name,
            Key = game.Key,
            Description = game.Description,
        };
    }

    private static string GenerateKey(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Guid.NewGuid().ToString("N");
        }

        var builder = new StringBuilder();
        var lastWasDash = false;
        foreach (var character in name.Trim().ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(character);
                lastWasDash = false;
                continue;
            }

            if (!lastWasDash)
            {
                builder.Append('-');
                lastWasDash = true;
            }
        }

        var key = builder.ToString().Trim('-');
        return string.IsNullOrWhiteSpace(key) ? Guid.NewGuid().ToString("N") : key;
    }

    private static string BuildFileName(string gameName)
    {
        var safeName = new string([.. gameName.Where(character => !Path.GetInvalidFileNameChars().Contains(character))]);

        if (string.IsNullOrWhiteSpace(safeName))
        {
            safeName = "game";
        }

        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        return $"{safeName}_{timestamp}.txt";
    }
}
