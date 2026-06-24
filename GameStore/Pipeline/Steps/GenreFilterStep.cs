namespace GameStore.Pipeline.Steps;

public class GenreFilterStep : IGameFilterStep
{
    public void Execute(GameFilterContext context)
    {
        if (context.Filter.GenreIds.Count == 0)
        {
            return;
        }

        context.Query = context.Query
            .Where(g => g.GameGenres.Any(gg => context.Filter.GenreIds.Contains(gg.GenreId)));
    }
}
