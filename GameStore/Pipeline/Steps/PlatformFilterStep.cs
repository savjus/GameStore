namespace GameStore.Pipeline.Steps;

public class PlatformFilterStep : IGameFilterStep
{
    public void Execute(GameFilterContext context)
    {
        if (context.Filter.PlatformIds.Count == 0)
        {
            return;
        }

        context.Query = context.Query
            .Where(g => g.GamePlatforms.Any(gp => context.Filter.PlatformIds.Contains(gp.PlatformId)));
    }
}
