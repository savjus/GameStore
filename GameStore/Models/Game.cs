namespace GameStore.Models;

public class Game
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Key { get; set; }

    public string Description { get; set; }
}