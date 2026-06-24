namespace GameStore.Models.Dtos;

public class GameResponseDto
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Key { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int UnitInStock { get; set; }

    public int Discount { get; set; }

    public int ViewCount { get; set; }

    public int CommentsCount { get; set; }

    public DateTime CreatedAt { get; set; }
}
