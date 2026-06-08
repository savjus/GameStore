using System.Text.Json;
using GameStore.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace GameStore.Tests.Middleware;

public class GlobalExceptionMiddlewareTests
{
    [Fact]
    public async Task Invoke_ReturnsProblemDetails_OnException()
    {
        var logger = NullLogger<GlobalExceptionHandlingMiddleware>.Instance;
        var middleware = new GlobalExceptionHandlingMiddleware(_ => throw new Exception("boom"), logger);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", context.Response.ContentType);

        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        Assert.Equal("An internal server error occurred", document.RootElement.GetProperty("message").GetString());
    }
}
