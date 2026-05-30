using GameStore.Data;
using GameStore.Models;
using GameStore.Models.Dtos;
using GameStore.Repositories;
using GameStore.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace GameStore.Tests.Services;

public class PublisherServiceTests
{
    [Fact]
    public async Task AddPublisherAsync_ReturnsBadRequest_WhenNameMissing()
    {
        var request = new AddPublisherRequest
        {
            Publisher = new PublisherCreateDto
            {
                CompanyName = " ",
                HomePage = "https://example.test",
                Description = "desc",
            },
        };

        var service = CreateService();

        var result = await service.AddPublisherAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("Publisher company name is required.", result.Error);
    }

    [Fact]
    public async Task AddPublisherAsync_ReturnsConflict_WhenNameExists()
    {
        var request = new AddPublisherRequest
        {
            Publisher = new PublisherCreateDto
            {
                CompanyName = "Publisher",
                HomePage = "https://example.test",
                Description = "desc",
            },
        };

        var publisherRepository = new Mock<IPublisherRepository>();
        publisherRepository.Setup(repo => repo.CompanyNameExistsAsync("Publisher", null))
            .ReturnsAsync(true);

        var service = CreateService(publisherRepository);

        var result = await service.AddPublisherAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);
        Assert.Equal("A publisher with the same company name already exists.", result.Error);
    }

    [Fact]
    public async Task AddPublisherAsync_ReturnsCreated_WhenValid()
    {
        var request = new AddPublisherRequest
        {
            Publisher = new PublisherCreateDto
            {
                CompanyName = "Publisher",
                HomePage = "https://example.test",
                Description = "desc",
            },
        };

        var publisherRepository = new Mock<IPublisherRepository>();
        publisherRepository.Setup(repo => repo.AddAsync(It.IsAny<Publisher>()))
            .Returns(Task.CompletedTask);

        var unitOfWork = CreateUnitOfWork(publisherRepository);
        unitOfWork.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(0);

        var service = new PublisherService(unitOfWork.Object);

        var result = await service.AddPublisherAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal("Publisher", result.Value!.CompanyName);
    }

    [Fact]
    public async Task GetPublisherByCompanyNameAsync_ReturnsNotFound_WhenMissing()
    {
        var publisherRepository = new Mock<IPublisherRepository>();
        publisherRepository.Setup(repo => repo.GetByCompanyNameAsync("missing"))
            .ReturnsAsync((Publisher?)null);

        var service = CreateService(publisherRepository);

        var result = await service.GetPublisherByCompanyNameAsync("missing");

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal("Publisher not found.", result.Error);
    }

    [Fact]
    public async Task UpdatePublisherAsync_ReturnsNotFound_WhenMissing()
    {
        var request = new UpdatePublisherRequest
        {
            Publisher = new PublisherUpdateDto
            {
                Id = Guid.NewGuid(),
                CompanyName = "Publisher",
                HomePage = "https://example.test",
                Description = "desc",
            },
        };

        var publisherRepository = new Mock<IPublisherRepository>();
        publisherRepository.Setup(repo => repo.GetByIdAsync(request.Publisher.Id))
            .ReturnsAsync((Publisher?)null);

        var service = CreateService(publisherRepository);

        var result = await service.UpdatePublisherAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal("Publisher not found.", result.Error);
    }

    private static Mock<IUnitOfWork> CreateUnitOfWork(Mock<IPublisherRepository>? publisherRepository = null)
    {
        var publisherRepo = publisherRepository ?? new Mock<IPublisherRepository>();

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(u => u.Publishers).Returns(publisherRepo.Object);
        return unitOfWork;
    }

    private static PublisherService CreateService(Mock<IPublisherRepository>? publisherRepository = null)
    {
        var unitOfWork = CreateUnitOfWork(publisherRepository);
        return new PublisherService(unitOfWork.Object);
    }
}
