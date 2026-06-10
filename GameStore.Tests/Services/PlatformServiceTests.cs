using GameStore.Data;
using GameStore.Models;
using GameStore.Models.Dtos;
using GameStore.Repositories;
using GameStore.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace GameStore.Tests.Services;

public class PlatformServiceTests
{
    [Fact]
    public async Task AddPlatformAsync_ReturnsBadRequest_WhenTypeMissing()
    {
        var request = new AddPlatformRequest
        {
            Platform = new PlatformCreateDto { Type = " " },
        };

        var service = CreateService();

        var result = await service.AddPlatformAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("Platform type is required.", result.Error);
    }

    [Fact]
    public async Task GetPlatformByIdAsync_ReturnsNotFound_WhenMissing()
    {
        var platformId = Guid.NewGuid();
        var platformRepository = new Mock<IPlatformRepository>();
        platformRepository.Setup(repo => repo.GetByIdAsync(platformId))
            .ReturnsAsync((Platform?)null);

        var service = CreateService(platformRepository);

        var result = await service.GetPlatformByIdAsync(platformId);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal("Platform not found.", result.Error);
    }

    [Fact]
    public async Task UpdatePlatformAsync_ReturnsOk_WhenUpdated()
    {
        var platformId = Guid.NewGuid();

        var platform = new Platform
        {
            Id = platformId,
            Type = "Old",
        };

        var request = new UpdatePlatformRequest
        {
            Platform = new PlatformUpdateDto
            {
                Id = platformId,
                Type = "New",
            },
        };

        var platformRepository = new Mock<IPlatformRepository>();

        platformRepository
            .Setup(repo => repo.GetByIdTrackedAsync(platformId))
            .ReturnsAsync(platform);

        platformRepository
            .Setup(repo => repo.TypeExistsAsync("New", platformId))
            .ReturnsAsync(false);

        var unitOfWork = CreateUnitOfWork(platformRepository);

        unitOfWork.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var service = new PlatformService(unitOfWork.Object);

        var result = await service.UpdatePlatformAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal("New", result.Value!.Type);
    }

    [Fact]
    public async Task DeletePlatformAsync_ReturnsBadRequest_WhenIdMissing()
    {
        var service = CreateService();

        var result = await service.DeletePlatformAsync(Guid.Empty);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("Platform id is required.", result.Error);
    }

    private static Mock<IUnitOfWork> CreateUnitOfWork(Mock<IPlatformRepository>? platformRepository = null)
    {
        var platformRepo = platformRepository ?? new Mock<IPlatformRepository>();

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(u => u.Platforms).Returns(platformRepo.Object);
        return unitOfWork;
    }

    private static PlatformService CreateService(Mock<IPlatformRepository>? platformRepository = null)
    {
        var unitOfWork = CreateUnitOfWork(platformRepository);
        return new PlatformService(unitOfWork.Object);
    }
}
