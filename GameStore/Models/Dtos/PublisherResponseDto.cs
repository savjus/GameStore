namespace GameStore.Models.Dtos;

public class PublisherResponseDto
{
    public Guid Id { get; set; }

    public required string CompanyName { get; set; }

    public required string Description { get; set; }

    public required string HomePage { get; set; }
}
