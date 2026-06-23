using GameStore.Models;
using GameStore.Models.Dtos;

namespace GameStore.Pipeline;

public class GameFilterContext(IQueryable<Game> query, GameFilterRequest filter)
{
    public IQueryable<Game> Query { get; set; } = query;

    public GameFilterRequest Filter { get; set; } = filter;

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}
