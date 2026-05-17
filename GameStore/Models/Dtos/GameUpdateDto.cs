namespace GameStore.Models.Dtos;

public class GameUpdateDto
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Key { get; set; }

    public string? Description { get; set; }
}
