using System.Globalization;
using GameStore.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace GameStore.Middleware;

public class TotalGamesCountHeaderMiddleware(RequestDelegate next)
{
    private const string HeaderName = "x-total-numbers-of-games";
    private const string CacheKey = "games-total-count";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(1);

    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context, IGameRepository gameRepository, IMemoryCache cache)
    {
        if (!ShouldApplyHeader(context.Request.Path))
        {
            await _next(context);
            return;
        }

        context.Response.OnStarting(async () =>
        {
            var count = await GetTotalCountAsync(gameRepository, cache);
            context.Response.Headers[HeaderName] = count.ToString(CultureInfo.InvariantCulture);
        });

        await _next(context);
    }

    private static bool ShouldApplyHeader(PathString path)
    {
        return path.StartsWithSegments("/games", StringComparison.OrdinalIgnoreCase)
            || path.StartsWithSegments("/genres", StringComparison.OrdinalIgnoreCase)
            || path.StartsWithSegments("/platforms", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<int> GetTotalCountAsync(IGameRepository gameRepository, IMemoryCache cache)
    {
        if (cache.TryGetValue(CacheKey, out int cachedCount))
        {
            return cachedCount;
        }

        var total = await gameRepository.GetTotalCountAsync();
        cache.Set(CacheKey, total, CacheDuration);
        return total;
    }
}
