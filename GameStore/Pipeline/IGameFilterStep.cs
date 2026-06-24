namespace GameStore.Pipeline;

public interface IGameFilterStep
{
    void Execute(GameFilterContext context);
}
