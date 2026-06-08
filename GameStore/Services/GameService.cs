using System.Text;
using System.Text.Json;
using GameStore.Data;
using GameStore.Models;
using GameStore.Models.Dtos;

namespace GameStore.Services;

public class GameService(IUnitOfWork unitOfWork) : IGameService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ServiceResult<GameResponseDto>> GetGameByKeyAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status400BadRequest,
                "Game key is required.");
        }

        var game = await _unitOfWork.Games.GetByKeyAsync(key);
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

        var game = await _unitOfWork.Games.GetByIdAsync(id);
        return game == null
            ? ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status404NotFound,
                "Game not found.")
            : ServiceResult.Success(MapToResponse(game), StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<List<GameResponseDto>>> GetAllGamesAsync()
    {
        var games = await _unitOfWork.Games.GetAllAsync();
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

        var platformExists = await _unitOfWork.Platforms.ExistsAsync(platformId);
        if (!platformExists)
        {
            return ServiceResult.Fail<List<GameResponseDto>>(
                StatusCodes.Status404NotFound,
                "Platform not found.");
        }

        var games = await _unitOfWork.Games.GetByPlatformIdAsync(platformId);
        var response = games.Select(MapToResponse).ToList();
        return ServiceResult.Success(response, StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<List<GenreResponseDto>>> GetGenresByGameKeyAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return ServiceResult.Fail<List<GenreResponseDto>>(
                StatusCodes.Status400BadRequest,
                "Game key is required.");
        }

        var genres = await _unitOfWork.Genres.GetByGameKeyAsync(key) ?? [];
        var response = genres.Select(MapToResponseGenre).ToList();
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

        var genreExists = await _unitOfWork.Genres.ExistsAsync(genreId);
        if (!genreExists)
        {
            return ServiceResult.Fail<List<GameResponseDto>>(
                StatusCodes.Status404NotFound,
                "Genre not found.");
        }

        var games = await _unitOfWork.Games.GetByGenreIdAsync(genreId);
        var response = games.Select(MapToResponse).ToList();
        return ServiceResult.Success(response, StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<List<PlatformResponseDto>>> GetPlatformsByGameKeyAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return ServiceResult.Fail<List<PlatformResponseDto>>(
                StatusCodes.Status400BadRequest,
                "Game key is required.");
        }

        var platforms = await _unitOfWork.Platforms.GetByGameKeyAsync(key);
        var response = platforms.Select(MapToResponsePlatform).ToList();
        return ServiceResult.Success(response, StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<GameResponseDto>> AddGameAsync(AddGameRequest request)
    {
        if (request.Publisher == Guid.Empty)
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status400BadRequest,
                "Publisher id is required.");
        }

        var publisher = await _unitOfWork.Publishers.GetByIdAsync(request.Publisher);
        if (publisher == null)
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status400BadRequest,
                "Publisher does not exist.");
        }

        var gameName = request.Game.Name.Trim();
        var gameKey = string.IsNullOrWhiteSpace(request.Game.Key)
            ? GenerateKey(gameName)
            : request.Game.Key.Trim();

        var keyExists = await _unitOfWork.Games.KeyExistsAsync(gameKey);
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
            Price = request.Game.Price,
            UnitInStock = request.Game.UnitInStock,
            Discount = request.Game.Discount,
            PublisherId = publisher.Id,
        };

        if (request.Genres.Count > 0)
        {
            var distinctGenreIds = request.Genres.Distinct().ToList();
            var existingGenres = await _unitOfWork.Genres.GetByIdsAsync(distinctGenreIds);

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
                });
            }
        }

        if (request.Platforms.Count > 0)
        {
            var distinctPlatformIds = request.Platforms.Distinct().ToList();
            var existingPlatforms = await _unitOfWork.Platforms.GetByIdsAsync(distinctPlatformIds);

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
                });
            }
        }

        await _unitOfWork.Games.AddAsync(game);
        await _unitOfWork.SaveChangesAsync();

        var response = new GameResponseDto
        {
            Id = game.Id,
            Name = game.Name,
            Key = game.Key,
            Description = game.Description,
            Price = game.Price,
            UnitInStock = game.UnitInStock,
            Discount = game.Discount,
        };

        return ServiceResult.Success(response, StatusCodes.Status201Created);
    }

    public async Task<ServiceResult<GameResponseDto>> UpdateGameAsync(UpdateGameRequest request)
    {
        if (request.Publisher == Guid.Empty)
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status400BadRequest,
                "Publisher id is required.");
        }

        if (request.Game.Id == Guid.Empty)
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status400BadRequest,
                "Game id is required.");
        }

        var game = await _unitOfWork.Games.GetByIdWithLinksAsync(request.Game.Id);
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

        var keyExists = await _unitOfWork.Games.KeyExistsAsync(gameKey, game.Id);
        if (keyExists)
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status409Conflict,
                $"Game key '{gameKey}' already exists.");
        }

        var publisher = await _unitOfWork.Publishers.GetByIdAsync(request.Publisher);
        if (publisher == null)
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status400BadRequest,
                "Publisher does not exist.");
        }

        game.Name = gameName;
        game.Key = gameKey;
        game.Description = request.Game.Description?.Trim();
        game.Price = request.Game.Price;
        game.UnitInStock = request.Game.UnitInStock;
        game.Discount = request.Game.Discount;
        game.PublisherId = publisher.Id;

        var distinctGenreIds = request.Genres.Distinct().ToList();
        var existingGenres = distinctGenreIds.Count == 0
            ? []
            : await _unitOfWork.Genres.GetByIdsAsync(distinctGenreIds);

        if (existingGenres.Count != distinctGenreIds.Count)
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status400BadRequest,
                "One or more genres do not exist.");
        }

        var distinctPlatformIds = request.Platforms.Distinct().ToList();
        var existingPlatforms = distinctPlatformIds.Count == 0
            ? []
            : await _unitOfWork.Platforms.GetByIdsAsync(distinctPlatformIds);

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
            });
        }

        await _unitOfWork.SaveChangesAsync();

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

        var game = await _unitOfWork.Games.GetByKeyAsync(key);
        if (game == null)
        {
            return ServiceResult.Fail<GameResponseDto>(
                StatusCodes.Status404NotFound,
                "Game not found.");
        }

        await _unitOfWork.Games.DeleteAsync(game);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(MapToResponse(game), StatusCodes.Status204NoContent);
    }

    public async Task<ServiceResult<GameFileDto>> GetGameFileAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return ServiceResult.Fail<GameFileDto>(
                StatusCodes.Status400BadRequest,
                "Game key is required.");
        }

        var game = await _unitOfWork.Games.GetByKeyAsync(key);
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
            Price = game.Price,
            UnitInStock = game.UnitInStock,
            Discount = game.Discount,
        };
    }

    private static GenreResponseDto MapToResponseGenre(Genre genre)
    {
        return new GenreResponseDto
        {
            Id = genre.Id,
            Name = genre.Name,
            ParentGenreId = genre.ParentGenreId,
        };
    }

    private static PlatformResponseDto MapToResponsePlatform(Platform platform)
    {
        return new PlatformResponseDto
        {
            Id = platform.Id,
            Type = platform.Type,
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
