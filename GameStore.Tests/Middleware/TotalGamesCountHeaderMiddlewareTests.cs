using GameStore.Middleware;
using GameStore.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace GameStore.Tests.Middleware;

public class TotalGamesCountHeaderMiddlewareTests
{
    [Fact]
    public async Task Invoke_AddsHeaderWithTotalCount()
    {
        var repository = new Mock<IGameRepository>();
        repository.Setup(repo => repo.GetTotalCountAsync())
            .ReturnsAsync(5);

        using var cache = new MemoryCache(new MemoryCacheOptions());
        var middleware = new TotalGamesCountHeaderMiddleware(async context =>
        {
            await context.Response.WriteAsync("ok");
        });

        var headerValue = await InvokeMiddlewareAsync(middleware, repository.Object, cache);

        Assert.Equal("5", headerValue);
        repository.Verify(repo => repo.GetTotalCountAsync(), Times.Once);
    }

    [Fact]
    public async Task Invoke_UsesCache_ForSubsequentRequests()
    {
        var repository = new Mock<IGameRepository>();
        repository.Setup(repo => repo.GetTotalCountAsync())
            .ReturnsAsync(9);

        using var cache = new MemoryCache(new MemoryCacheOptions());
        var middleware = new TotalGamesCountHeaderMiddleware(async context =>
        {
            await context.Response.WriteAsync("ok");
        });

        var firstHeader = await InvokeMiddlewareAsync(middleware, repository.Object, cache);
        var secondHeader = await InvokeMiddlewareAsync(middleware, repository.Object, cache);

        Assert.Equal("9", firstHeader);
        Assert.Equal("9", secondHeader);
        repository.Verify(repo => repo.GetTotalCountAsync(), Times.Once);
    }

    private static async Task<string?> InvokeMiddlewareAsync(
        TotalGamesCountHeaderMiddleware middleware,
        IGameRepository repository,
        IMemoryCache cache)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.Invoke(context, repository, cache);

        context.Response.Headers.TryGetValue("x-total-numbers-of-games", out var headerValue);
        return headerValue.ToString();
    }
}
