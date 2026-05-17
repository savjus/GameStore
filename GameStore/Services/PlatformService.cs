using GameStore.Data;
using GameStore.Models;
using GameStore.Models.Dtos;

namespace GameStore.Services;

public class PlatformService(IUnitOfWork unitOfWork) : IPlatformService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ServiceResult<PlatformResponseDto>> AddPlatformAsync(AddPlatformRequest request)
    {
        if (request.Platform == null || string.IsNullOrWhiteSpace(request.Platform.Type))
        {
            return ServiceResult.Fail<PlatformResponseDto>(
                StatusCodes.Status400BadRequest,
                "Platform type is required.");
        }

        var trimmedType = request.Platform.Type.Trim();
        var typeExists = await _unitOfWork.Platforms.TypeExistsAsync(trimmedType);
        if (typeExists)
        {
            return ServiceResult.Fail<PlatformResponseDto>(
                StatusCodes.Status409Conflict,
                "A platform with the same type already exists.");
        }

        var platform = new Platform
        {
            Id = Guid.NewGuid(),
            Type = trimmedType,
        };

        await _unitOfWork.Platforms.AddAsync(platform);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(MapToResponse(platform), StatusCodes.Status201Created);
    }

    public async Task<ServiceResult<PlatformResponseDto>> GetPlatformByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return ServiceResult.Fail<PlatformResponseDto>(
                StatusCodes.Status400BadRequest,
                "Platform id is required.");
        }

        var platform = await _unitOfWork.Platforms.GetByIdAsync(id);
        if (platform == null)
        {
            return ServiceResult.Fail<PlatformResponseDto>(
                StatusCodes.Status404NotFound,
                "Platform not found.");
        }

        return ServiceResult.Success(MapToResponse(platform), StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<List<PlatformResponseDto>>> GetAllPlatformsAsync()
    {
        var platforms = await _unitOfWork.Platforms.GetAllAsync();
        var response = platforms.Select(MapToResponse).ToList();
        return ServiceResult.Success(response, StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<PlatformResponseDto>> UpdatePlatformAsync(UpdatePlatformRequest request)
    {
        if (request.Platform == null || request.Platform.Id == Guid.Empty)
        {
            return ServiceResult.Fail<PlatformResponseDto>(
                StatusCodes.Status400BadRequest,
                "Platform id is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Platform.Type))
        {
            return ServiceResult.Fail<PlatformResponseDto>(
                StatusCodes.Status400BadRequest,
                "Platform type is required.");
        }

        var platform = await _unitOfWork.Platforms.GetByIdAsync(request.Platform.Id);
        if (platform == null)
        {
            return ServiceResult.Fail<PlatformResponseDto>(
                StatusCodes.Status404NotFound,
                "Platform not found.");
        }

        var trimmedType = request.Platform.Type.Trim();
        var typeExists = await _unitOfWork.Platforms.TypeExistsAsync(trimmedType, request.Platform.Id);
        if (typeExists)
        {
            return ServiceResult.Fail<PlatformResponseDto>(
                StatusCodes.Status409Conflict,
                "A platform with the same type already exists.");
        }

        platform.Type = trimmedType;
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(MapToResponse(platform), StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<PlatformResponseDto>> DeletePlatformAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return ServiceResult.Fail<PlatformResponseDto>(
                StatusCodes.Status400BadRequest,
                "Platform id is required.");
        }

        var platform = await _unitOfWork.Platforms.GetByIdAsync(id);
        if (platform == null)
        {
            return ServiceResult.Fail<PlatformResponseDto>(
                StatusCodes.Status404NotFound,
                "Platform not found.");
        }

        await _unitOfWork.Platforms.DeleteAsync(platform);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(MapToResponse(platform), StatusCodes.Status204NoContent);
    }

    private static PlatformResponseDto MapToResponse(Platform platform)
    {
        return new PlatformResponseDto
        {
            Id = platform.Id,
            Type = platform.Type,
        };
    }
}
