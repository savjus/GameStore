using GameStore.Models.Dtos;

namespace GameStore.Services;

public interface IGenreService
{
    Task<ServiceResult<GenreResponseDto>> AddGenreAsync(AddGenreRequest request);

    Task<ServiceResult<GenreResponseDto>> UpdateGenreAsync(UpdateGenreRequest request);

    Task<ServiceResult<GenreResponseDto>> DeleteGenreAsync(Guid id);

    Task<ServiceResult<GenreResponseDto>> GetGenreByIdAsync(Guid id);

    Task<ServiceResult<List<GenreResponseDto>>> GetAllGenresAsync();

    Task<ServiceResult<List<GenreResponseDto>>> GetGenresByGameKeyAsync(string key);

    Task<ServiceResult<List<GenreResponseDto>>> GetGenresByParentIdAsync(Guid parentId);
}
