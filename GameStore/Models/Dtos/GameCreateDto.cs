namespace GameStore.Models.Dtos;

public class GameCreateDto
{
    public required string Name { get; set; }

    public string? Key { get; set; }

    public string? Description { get; set; }
}
