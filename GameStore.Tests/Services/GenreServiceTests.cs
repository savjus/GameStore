using GameStore.Models;
using GameStore.Models.Dtos;
using GameStore.Repositories;
using GameStore.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace GameStore.Tests.Services;

public class GenreServiceTests
{
    [Fact]
    public async Task AddGenreAsync_ReturnsBadRequest_WhenNameMissing()
    {
        var request = new AddGenreRequest
        {
            Genre = new GenreCreateDto { Name = " " },
        };

        var service = CreateService();

        var result = await service.AddGenreAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("Genre name is required.", result.Error);
    }

    [Fact]
    public async Task AddGenreAsync_ReturnsBadRequest_WhenParentMissing()
    {
        var parentId = Guid.NewGuid();
        var request = new AddGenreRequest
        {
            Genre = new GenreCreateDto { Name = "Action", ParentGenreId = parentId },
        };

        var genreRepository = new Mock<IGenreRepository>();
        genreRepository.Setup(repo => repo.ExistsAsync(parentId))
            .ReturnsAsync(false);

        var service = CreateService(genreRepository);

        var result = await service.AddGenreAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("Parent genre does not exist.", result.Error);
    }

    [Fact]
    public async Task AddGenreAsync_ReturnsCreated_WhenValid()
    {
        var request = new AddGenreRequest
        {
            Genre = new GenreCreateDto { Name = "Action" },
        };

        var genreRepository = new Mock<IGenreRepository>();
        genreRepository.Setup(repo => repo.AddAsync(It.IsAny<Genre>()))
            .Returns(Task.CompletedTask);
        genreRepository.Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = CreateService(genreRepository);

        var result = await service.AddGenreAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal("Action", result.Value!.Name);
    }

    [Fact]
    public async Task UpdateGenreAsync_ReturnsNotFound_WhenMissing()
    {
        var request = new UpdateGenreRequest
        {
            Genre = new GenreUpdateDto { Id = Guid.NewGuid(), Name = "Action" },
        };

        var genreRepository = new Mock<IGenreRepository>();
        genreRepository.Setup(repo => repo.GetByIdAsync(request.Genre.Id))
            .ReturnsAsync((Genre?)null);

        var service = CreateService(genreRepository);

        var result = await service.UpdateGenreAsync(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal("Genre not found.", result.Error);
    }

    [Fact]
    public async Task DeleteGenreAsync_ReturnsOk_WhenDeleted()
    {
        var genreId = Guid.NewGuid();
        var genre = new Genre { Id = genreId, Name = "Action" };

        var genreRepository = new Mock<IGenreRepository>();
        genreRepository.Setup(repo => repo.GetByIdAsync(genreId))
            .ReturnsAsync(genre);
        genreRepository.Setup(repo => repo.DeleteAsync(genre))
            .Returns(Task.CompletedTask);
        genreRepository.Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var service = CreateService(genreRepository);

        var result = await service.DeleteGenreAsync(genreId);

        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(genreId, result.Value!.Id);
    }

    private static GenreService CreateService(Mock<IGenreRepository>? genreRepository = null)
    {
        var repo = genreRepository ?? new Mock<IGenreRepository>();
        return new GenreService(repo.Object);
    }
}
