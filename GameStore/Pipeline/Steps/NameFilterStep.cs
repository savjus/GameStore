namespace GameStore.Pipeline.Steps;

public class NameFilterStep : IGameFilterStep
{
    private const int MinNameLength = 3;

    public void Execute(GameFilterContext context)
    {
        if (string.IsNullOrWhiteSpace(context.Filter.Name) || context.Filter.Name.Trim().Length < MinNameLength)
        {
            return;
        }

        var name = context.Filter.Name.Trim();
        context.Query = context.Query.Where(g => g.Name.Contains(name));
    }
}
