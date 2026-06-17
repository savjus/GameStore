using GameStore.Models.Dtos;

namespace GameStore.Services;

public interface IPublisherService
{
    Task<ServiceResult<PublisherResponseDto>> AddPublisherAsync(AddPublisherRequest request);

    Task<ServiceResult<PublisherResponseDto>> UpdatePublisherAsync(UpdatePublisherRequest request);

    Task<ServiceResult<PublisherResponseDto>> DeletePublisherAsync(Guid id);

    Task<ServiceResult<PublisherResponseDto>> GetPublisherByCompanyNameAsync(string companyName);

    Task<ServiceResult<List<PublisherResponseDto>>> GetAllPublishersAsync();

    Task<ServiceResult<PublisherResponseDto>> GetPublisherByGameKeyAsync(string key);

    Task<ServiceResult<List<GameResponseDto>>> GetGamesByPublisherNameAsync(string companyName);
}
