using GameStore.Pipeline.Steps;

namespace GameStore.Pipeline;

public class GameFilterPipeline
{
    private readonly List<IGameFilterStep> _steps =
    [
        new GenreFilterStep(),
        new PlatformFilterStep(),
        new PublisherFilterStep(),
        new PriceRangeFilterStep(),
        new PublishDateFilterStep(),
        new NameFilterStep(),
        new SortStep(),
        new PaginationStep(),
    ];

    public void Execute(GameFilterContext context)
    {
        foreach (var step in _steps)
        {
            step.Execute(context);
        }
    }
}
