namespace GameStore.Models;

public class GameGenre
{
    public Guid GameId { get; set; }

    public Guid GenreId { get; set; }

    public required Game Game { get; set; }

    public required Genre Genre { get; set; }
}