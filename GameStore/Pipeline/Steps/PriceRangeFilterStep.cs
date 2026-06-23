namespace GameStore.Pipeline.Steps;

public class PriceRangeFilterStep : IGameFilterStep
{
    public void Execute(GameFilterContext context)
    {
        if (context.Filter.MinPrice.HasValue)
        {
            context.Query = context.Query.Where(g => g.Price >= context.Filter.MinPrice.Value);
        }

        if (context.Filter.MaxPrice.HasValue)
        {
            context.Query = context.Query.Where(g => g.Price <= context.Filter.MaxPrice.Value);
        }
    }
}
