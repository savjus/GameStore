using System.Text.Json;
using GameStore.Models.Dtos;

namespace GameStore.Middleware;

public class GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred");
            await HandleExceptionAsync(context, exception);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var errorResponse = new ErrorResponseDto
        {
            Message = "An unexpected error occurred",
        };

        switch (exception)
        {
            case ArgumentNullException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                errorResponse.Message = "Required field is missing or null.";
                break;

            case ArgumentException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                errorResponse.Message = "Invalid request data.";
                break;

            case InvalidOperationException:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                errorResponse.Message = "The request could not be completed because of a conflict.";
                break;

            case KeyNotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                errorResponse.Message = "Resource not found.";
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                errorResponse.Message = "An internal server error occurred";
                break;
        }

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsJsonAsync(errorResponse, options);
    }
}
