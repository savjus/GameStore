using GameStore.Data;
using GameStore.Models;
using GameStore.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Services;

public class PublisherService(IUnitOfWork unitOfWork) : IPublisherService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ServiceResult<PublisherResponseDto>> AddPublisherAsync(AddPublisherRequest request)
    {
        var companyName = request.Publisher.CompanyName.Trim();
        var homePage = request.Publisher.HomePage.Trim();
        var description = request.Publisher.Description.Trim();

        if (string.IsNullOrWhiteSpace(companyName))
        {
            return ServiceResult.Fail<PublisherResponseDto>(
                StatusCodes.Status400BadRequest,
                "Publisher company name is required.");
        }

        if (string.IsNullOrWhiteSpace(homePage))
        {
            return ServiceResult.Fail<PublisherResponseDto>(
                StatusCodes.Status400BadRequest,
                "Publisher home page is required.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return ServiceResult.Fail<PublisherResponseDto>(
                StatusCodes.Status400BadRequest,
                "Publisher description is required.");
        }

        var nameExists = await _unitOfWork.Publishers.CompanyNameExistsAsync(companyName);
        if (nameExists)
        {
            return ServiceResult.Fail<PublisherResponseDto>(
                StatusCodes.Status409Conflict,
                "A publisher with the same company name already exists.");
        }

        var publisher = new Publisher
        {
            Id = Guid.NewGuid(),
            CompanyName = companyName,
            HomePage = homePage,
            Description = description,
        };

        await _unitOfWork.Publishers.AddAsync(publisher);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(MapToResponse(publisher), StatusCodes.Status201Created);
    }

    public async Task<ServiceResult<PublisherResponseDto>> UpdatePublisherAsync(UpdatePublisherRequest request)
    {
        if (request.Publisher.Id == Guid.Empty)
        {
            return ServiceResult.Fail<PublisherResponseDto>(
                StatusCodes.Status400BadRequest,
                "Publisher id is required.");
        }

        var publisher = await _unitOfWork.Publishers.GetByIdTrackedAsync(request.Publisher.Id);
        if (publisher == null)
        {
            return ServiceResult.Fail<PublisherResponseDto>(
                StatusCodes.Status404NotFound,
                "Publisher not found.");
        }

        var companyName = request.Publisher.CompanyName.Trim();
        var homePage = request.Publisher.HomePage.Trim();
        var description = request.Publisher.Description.Trim();

        if (string.IsNullOrWhiteSpace(companyName))
        {
            return ServiceResult.Fail<PublisherResponseDto>(
                StatusCodes.Status400BadRequest,
                "Publisher company name is required.");
        }

        if (string.IsNullOrWhiteSpace(homePage))
        {
            return ServiceResult.Fail<PublisherResponseDto>(
                StatusCodes.Status400BadRequest,
                "Publisher home page is required.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return ServiceResult.Fail<PublisherResponseDto>(
                StatusCodes.Status400BadRequest,
                "Publisher description is required.");
        }

        var nameExists = await _unitOfWork.Publishers.CompanyNameExistsAsync(companyName, request.Publisher.Id);
        if (nameExists)
        {
            return ServiceResult.Fail<PublisherResponseDto>(
                StatusCodes.Status409Conflict,
                "A publisher with the same company name already exists.");
        }

        publisher.CompanyName = companyName;
        publisher.HomePage = homePage;
        publisher.Description = description;

        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(MapToResponse(publisher), StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<PublisherResponseDto>> DeletePublisherAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return ServiceResult.Fail<PublisherResponseDto>(
                StatusCodes.Status400BadRequest,
                "Publisher id is required.");
        }

        var publisher = await _unitOfWork.Publishers.GetByIdAsync(id);
        if (publisher == null)
        {
            return ServiceResult.Fail<PublisherResponseDto>(
                StatusCodes.Status404NotFound,
                "Publisher not found.");
        }

        await _unitOfWork.Publishers.DeleteAsync(publisher);
        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return ServiceResult.Fail<PublisherResponseDto>(
                StatusCodes.Status409Conflict,
                "Cannot delete a publisher that is still referenced by games.");
        }

        return ServiceResult.Success(MapToResponse(publisher), StatusCodes.Status204NoContent);
    }

    public async Task<ServiceResult<PublisherResponseDto>> GetPublisherByCompanyNameAsync(string companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName))
        {
            return ServiceResult.Fail<PublisherResponseDto>(
                StatusCodes.Status400BadRequest,
                "Publisher company name is required.");
        }

        var publisher = await _unitOfWork.Publishers.GetByCompanyNameAsync(companyName.Trim());
        return publisher == null
            ? ServiceResult.Fail<PublisherResponseDto>(
                StatusCodes.Status404NotFound,
                "Publisher not found.")
            : ServiceResult.Success(MapToResponse(publisher), StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<List<PublisherResponseDto>>> GetAllPublishersAsync()
    {
        var publishers = await _unitOfWork.Publishers.GetAllAsync();
        var response = publishers.Select(MapToResponse).ToList();
        return ServiceResult.Success(response, StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<PublisherResponseDto>> GetPublisherByGameKeyAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return ServiceResult.Fail<PublisherResponseDto>(
                StatusCodes.Status400BadRequest,
                "Game key is required.");
        }

        var publisher = await _unitOfWork.Publishers.GetByGameKeyAsync(key.Trim());
        return publisher == null
            ? ServiceResult.Fail<PublisherResponseDto>(
                StatusCodes.Status404NotFound,
                "Publisher not found.")
            : ServiceResult.Success(MapToResponse(publisher), StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<List<GameResponseDto>>> GetGamesByPublisherNameAsync(string companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName))
        {
            return ServiceResult.Fail<List<GameResponseDto>>(
                StatusCodes.Status400BadRequest,
                "Publisher company name is required.");
        }

        var publisher = await _unitOfWork.Publishers.GetByCompanyNameAsync(companyName.Trim());
        if (publisher == null)
        {
            return ServiceResult.Fail<List<GameResponseDto>>(
                StatusCodes.Status404NotFound,
                "Publisher not found.");
        }

        var games = await _unitOfWork.Games.GetByPublisherIdAsync(publisher.Id);
        var response = games.Select(MapToGameResponse).ToList();
        return ServiceResult.Success(response, StatusCodes.Status200OK);
    }

    private static PublisherResponseDto MapToResponse(Publisher publisher)
    {
        return new PublisherResponseDto
        {
            Id = publisher.Id,
            CompanyName = publisher.CompanyName,
            Description = publisher.Description ?? string.Empty,
            HomePage = publisher.HomePage ?? string.Empty,
        };
    }

    private static GameResponseDto MapToGameResponse(Game game)
    {
        return new GameResponseDto
        {
            Id = game.Id,
            Name = game.Name,
            Key = game.Key,
            Description = game.Description,
        };
    }
}
