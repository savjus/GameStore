using System.Diagnostics;
using System.Text;

namespace GameStore.Middleware;

public sealed class RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestBody = await ReadRequestBodyAsync(context.Request);
        var originalBodyStream = context.Response.Body;

        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            context.Response.Body = originalBodyStream;
            responseBody.Position = 0;
            var responseText = await new StreamReader(responseBody, Encoding.UTF8, false, 1024, true)
                .ReadToEndAsync();
            responseBody.Position = 0;
            await responseBody.CopyToAsync(originalBodyStream);

            _logger.LogInformation(
                "HTTP {Method} {Path} from {Ip} responded {StatusCode} in {ElapsedMs}ms. Request: {RequestBody}. Response: {ResponseBody}.",
                context.Request.Method,
                GetTargetUrl(context.Request),
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                requestBody,
                responseText);
        }
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        if (request.ContentLength is null or 0)
        {
            return string.Empty;
        }

        request.EnableBuffering();
        using var reader = new StreamReader(request.Body, Encoding.UTF8, false, 1024, true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        return body;
    }

    private static string GetTargetUrl(HttpRequest request)
    {
        return string.Concat(request.Path.Value, request.QueryString.Value);
    }
}
