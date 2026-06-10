using GameStore.Models.Dtos;

namespace GameStore.Services;

public interface IPlatformService
{
    Task<ServiceResult<PlatformResponseDto>> AddPlatformAsync(AddPlatformRequest request);

    Task<ServiceResult<PlatformResponseDto>> GetPlatformByIdAsync(Guid id);

    Task<ServiceResult<List<PlatformResponseDto>>> GetAllPlatformsAsync();

    Task<ServiceResult<PlatformResponseDto>> UpdatePlatformAsync(UpdatePlatformRequest request);

    Task<ServiceResult<PlatformResponseDto>> DeletePlatformAsync(Guid id);
}
