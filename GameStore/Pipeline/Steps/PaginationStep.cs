namespace GameStore.Pipeline.Steps;

public class PaginationStep : IGameFilterStep
{
    public void Execute(GameFilterContext context)
    {
        var totalCount = context.Query.Count();
        context.TotalCount = totalCount;

        if (string.Equals(context.Filter.PageSize, "all", StringComparison.OrdinalIgnoreCase))
        {
            context.TotalPages = 1;
            return;
        }

        if (!int.TryParse(context.Filter.PageSize, out var pageSize) || pageSize <= 0)
        {
            pageSize = 10;
        }

        var currentPage = context.Filter.Page < 1 ? 1 : context.Filter.Page;
        context.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        context.Query = context.Query
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize);
    }
}
