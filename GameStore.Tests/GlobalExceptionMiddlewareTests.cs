using System.Text.Json;
using GameStore.Middleware;
using Microsoft.AspNetCore.Http;

namespace GameStore.Tests.Middleware;

public class GlobalExceptionMiddlewareTests
{
    [Fact]
    public async Task Invoke_ReturnsProblemDetails_OnException()
    {
        var middleware = new GlobalExceptionMiddleware(_ => throw new InvalidOperationException("boom"));
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.Invoke(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", context.Response.ContentType);

        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        Assert.Equal("An unexpected error occurred.", document.RootElement.GetProperty("title").GetString());
        Assert.Equal(500, document.RootElement.GetProperty("status").GetInt32());
    }
}
