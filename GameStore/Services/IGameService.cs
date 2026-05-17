using GameStore.Models.Dtos;

namespace GameStore.Services;

public interface IGameService
{
    Task<ServiceResult<GameResponseDto>> GetGameByKeyAsync(string key);

    Task<ServiceResult<GameResponseDto>> GetGameByIdAsync(Guid id);

    Task<ServiceResult<List<GameResponseDto>>> GetAllGamesAsync();

    Task<ServiceResult<List<GameResponseDto>>> GetGamesByPlatformAsync(Guid platformId);

    Task<ServiceResult<List<GameResponseDto>>> GetGamesByGenreAsync(Guid genreId);

    Task<ServiceResult<GameResponseDto>> AddGameAsync(AddGameRequest request);

    Task<ServiceResult<GameResponseDto>> UpdateGameAsync(UpdateGameRequest request);

    Task<ServiceResult<GameResponseDto>> DeleteGameAsync(string key);

    Task<ServiceResult<List<GenreResponseDto>>> GetGenresByGameKeyAsync(string key);

    Task<ServiceResult<List<PlatformResponseDto>>> GetPlatformsByGameKeyAsync(string key);

    Task<ServiceResult<GameFileDto>> GetGameFileAsync(string key);
}
