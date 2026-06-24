namespace GameStore.Pipeline.Steps;

public class PublishDateFilterStep : IGameFilterStep
{
    public void Execute(GameFilterContext context)
    {
        if (string.IsNullOrWhiteSpace(context.Filter.PublishDateFilter))
        {
            return;
        }

        var now = DateTime.UtcNow;
        DateTime? cutoff = context.Filter.PublishDateFilter switch
        {
            "last week" => now.AddDays(-7),
            "last month" => now.AddMonths(-1),
            "last year" => now.AddYears(-1),
            "2 years" => now.AddYears(-2),
            "3 years" => now.AddYears(-3),
            _ => null,
        };

        if (cutoff.HasValue)
        {
            context.Query = context.Query.Where(g => g.CreatedAt >= cutoff.Value);
        }
    }
}
