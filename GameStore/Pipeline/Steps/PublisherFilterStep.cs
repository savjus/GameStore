namespace GameStore.Pipeline.Steps;

public class PublisherFilterStep : IGameFilterStep
{
    public void Execute(GameFilterContext context)
    {
        if (context.Filter.PublisherIds.Count == 0)
        {
            return;
        }

        context.Query = context.Query
            .Where(g => context.Filter.PublisherIds.Contains(g.PublisherId));
    }
}
