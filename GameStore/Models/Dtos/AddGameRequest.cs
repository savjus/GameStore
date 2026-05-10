namespace GameStore.Models.Dtos;

public class AddGameRequest
{
    public required GameCreateDto Game { get; set; }

    public List<Guid> Genres { get; set; } = [];

    public List<Guid> Platforms { get; set; } = [];
}
