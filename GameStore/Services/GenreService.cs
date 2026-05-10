using GameStore.Data;
using GameStore.Models;
using GameStore.Models.Dtos;

namespace GameStore.Services;

public class GenreService(IUnitOfWork unitOfWork) : IGenreService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ServiceResult<GenreResponseDto>> AddGenreAsync(AddGenreRequest request)
    {
        if (request.Genre == null || string.IsNullOrWhiteSpace(request.Genre.Name))
        {
            return ServiceResult.Fail<GenreResponseDto>(
                StatusCodes.Status400BadRequest,
                "Genre name is required.");
        }

        if (request.Genre.ParentGenreId.HasValue)
        {
            var parentExists = await _unitOfWork.Genres.ExistsAsync(request.Genre.ParentGenreId.Value);
            if (!parentExists)
            {
                return ServiceResult.Fail<GenreResponseDto>(
                    StatusCodes.Status400BadRequest,
                    "Parent genre does not exist.");
            }
        }

        var genre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = request.Genre.Name.Trim(),
            ParentGenreId = request.Genre.ParentGenreId,
        };

        await _unitOfWork.Genres.AddAsync(genre);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(MapToResponse(genre), StatusCodes.Status201Created);
    }

    public async Task<ServiceResult<GenreResponseDto>> UpdateGenreAsync(UpdateGenreRequest request)
    {
        if (request.Genre == null || request.Genre.Id == Guid.Empty)
        {
            return ServiceResult.Fail<GenreResponseDto>(
                StatusCodes.Status400BadRequest,
                "Genre id is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Genre.Name))
        {
            return ServiceResult.Fail<GenreResponseDto>(
                StatusCodes.Status400BadRequest,
                "Genre name is required.");
        }

        var genre = await _unitOfWork.Genres.GetByIdAsync(request.Genre.Id);
        if (genre == null)
        {
            return ServiceResult.Fail<GenreResponseDto>(
                StatusCodes.Status404NotFound,
                "Genre not found.");
        }

        if (request.Genre.ParentGenreId.HasValue)
        {
            var parentExists = await _unitOfWork.Genres.ExistsAsync(request.Genre.ParentGenreId.Value);
            if (!parentExists)
            {
                return ServiceResult.Fail<GenreResponseDto>(
                    StatusCodes.Status400BadRequest,
                    "Parent genre does not exist.");
            }
        }

        genre.Name = request.Genre.Name.Trim();
        genre.ParentGenreId = request.Genre.ParentGenreId;

        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(MapToResponse(genre), StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<GenreResponseDto>> DeleteGenreAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return ServiceResult.Fail<GenreResponseDto>(
                StatusCodes.Status400BadRequest,
                "Genre id is required.");
        }

        var genre = await _unitOfWork.Genres.GetByIdAsync(id);
        if (genre == null)
        {
            return ServiceResult.Fail<GenreResponseDto>(
                StatusCodes.Status404NotFound,
                "Genre not found.");
        }

        await _unitOfWork.Genres.DeleteAsync(genre);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(MapToResponse(genre), StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<GenreResponseDto>> GetGenreByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return ServiceResult.Fail<GenreResponseDto>(
                StatusCodes.Status400BadRequest,
                "Genre id is required.");
        }

        var genre = await _unitOfWork.Genres.GetByIdAsync(id);
        return genre == null
            ? ServiceResult.Fail<GenreResponseDto>(
                StatusCodes.Status404NotFound,
                "Genre not found.")
            : ServiceResult.Success(MapToResponse(genre), StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<List<GenreResponseDto>>> GetAllGenresAsync()
    {
        var genres = await _unitOfWork.Genres.GetAllAsync();
        var response = genres.Select(MapToResponse).ToList();
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
        var response = genres.Select(MapToResponse).ToList();
        return ServiceResult.Success(response, StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<List<GenreResponseDto>>> GetGenresByParentIdAsync(Guid parentId)
    {
        if (parentId == Guid.Empty)
        {
            return ServiceResult.Fail<List<GenreResponseDto>>(
                StatusCodes.Status400BadRequest,
                "Parent genre id is required.");
        }

        var genres = await _unitOfWork.Genres.GetByParentIdAsync(parentId) ?? [];
        var response = genres.Select(MapToResponse).ToList();
        return ServiceResult.Success(response, StatusCodes.Status200OK);
    }

    private static GenreResponseDto MapToResponse(Genre genre)
    {
        return new GenreResponseDto
        {
            Id = genre.Id,
            Name = genre.Name,
            ParentGenreId = genre.ParentGenreId,
        };
    }
}
