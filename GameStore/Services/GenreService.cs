using GameStore.Data;
using GameStore.Models;
using GameStore.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Services;

public class GenreService(IUnitOfWork unitOfWork) : IGenreService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ServiceResult<GenreResponseDto>> AddGenreAsync(AddGenreRequest request)
    {
        var trimmedName = request.Genre.Name.Trim();
        if (string.IsNullOrWhiteSpace(trimmedName))
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

        var nameExists = await _unitOfWork.Genres.NameExistsAsync(trimmedName);
        if (nameExists)
        {
            return ServiceResult.Fail<GenreResponseDto>(
                StatusCodes.Status409Conflict,
                "A genre with the same name already exists.");
        }

        var genre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = trimmedName,
            ParentGenreId = request.Genre.ParentGenreId,
        };

        await _unitOfWork.Genres.AddAsync(genre);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(MapToResponse(genre), StatusCodes.Status201Created);
    }

    public async Task<ServiceResult<GenreResponseDto>> UpdateGenreAsync(UpdateGenreRequest request)
    {
        if (request.Genre.Id == Guid.Empty)
        {
            return ServiceResult.Fail<GenreResponseDto>(
                StatusCodes.Status400BadRequest,
                "Genre id is required.");
        }

        var genre = await _unitOfWork.Genres.GetByIdAsync(request.Genre.Id);
        if (genre == null)
        {
            return ServiceResult.Fail<GenreResponseDto>(
                StatusCodes.Status404NotFound,
                "Genre not found.");
        }

        var trimmedName = request.Genre.Name.Trim();
        var nameExists = await _unitOfWork.Genres.NameExistsAsync(trimmedName, request.Genre.Id);
        if (nameExists)
        {
            return ServiceResult.Fail<GenreResponseDto>(
                StatusCodes.Status409Conflict,
                "A genre with the same name already exists.");
        }

        if (request.Genre.ParentGenreId.HasValue)
        {
            if (request.Genre.ParentGenreId.Value == request.Genre.Id)
            {
                return ServiceResult.Fail<GenreResponseDto>(
                    StatusCodes.Status400BadRequest,
                    "A genre cannot be its own parent.");
            }

            var parentExists = await _unitOfWork.Genres.ExistsAsync(request.Genre.ParentGenreId.Value);
            if (!parentExists)
            {
                return ServiceResult.Fail<GenreResponseDto>(
                    StatusCodes.Status400BadRequest,
                    "Parent genre does not exist.");
            }

            var hasCycle = await CreatesGenreHierarchyCycleAsync(request.Genre.Id, request.Genre.ParentGenreId.Value);
            if (hasCycle)
            {
                return ServiceResult.Fail<GenreResponseDto>(
                    StatusCodes.Status400BadRequest,
                    "Updating the parent genre would create a cycle.");
            }
        }

        genre.Name = trimmedName;
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

        var childGenres = await _unitOfWork.Genres.GetByParentIdAsync(id);
        if (childGenres != null && childGenres.Count > 0)
        {
            return ServiceResult.Fail<GenreResponseDto>(
                StatusCodes.Status409Conflict,
                "Cannot delete a parent genre while child genres still exist.");
        }

        await _unitOfWork.Genres.DeleteAsync(genre);
        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ServiceResult.Fail<GenreResponseDto>(
                StatusCodes.Status409Conflict,
                "Cannot delete a genre that is still referenced by child genres.");
        }

        return ServiceResult.Success(MapToResponse(genre), StatusCodes.Status204NoContent);
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

    private async Task<bool> CreatesGenreHierarchyCycleAsync(Guid genreId, Guid newParentId)
    {
        var current = await _unitOfWork.Genres.GetByIdAsync(newParentId);
        while (current != null)
        {
            if (current.Id == genreId)
            {
                return true;
            }

            if (!current.ParentGenreId.HasValue)
            {
                break;
            }

            current = await _unitOfWork.Genres.GetByIdAsync(current.ParentGenreId.Value);
        }

        return false;
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
