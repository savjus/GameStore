namespace GameStore.Models.Dtos;

public class UpdateGameRequest
{
    public required GameUpdateDto Game { get; set; }

    public List<Guid> Genres { get; set; } = [];

    public List<Guid> Platforms { get; set; } = [];
}
