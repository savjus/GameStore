namespace GameStore.Models.Dtos;

public class PlatformUpdateDto
{
    public Guid Id { get; set; }

    public required string Type { get; set; }
}
