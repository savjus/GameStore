namespace GameStore.Pipeline.Steps;

public class SortStep : IGameFilterStep
{
    public void Execute(GameFilterContext context)
    {
        context.Query = context.Filter.SortBy switch
        {
            "Most popular" => context.Query.OrderByDescending(g => g.ViewCount),
            "Most commented" => context.Query.OrderByDescending(g => g.Comments.Count),
            "Price ASC" => context.Query.OrderBy(g => g.Price),
            "Price DESC" => context.Query.OrderByDescending(g => g.Price),
            "New" => context.Query.OrderByDescending(g => g.CreatedAt),
            _ => context.Query.OrderBy(g => g.Name),
        };
    }
}
