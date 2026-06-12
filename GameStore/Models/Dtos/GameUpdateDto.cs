using System.ComponentModel.DataAnnotations;

namespace GameStore.Models.Dtos;

public class GameUpdateDto
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public required string Name { get; set; }

    [MaxLength(100)]
    public string? Key { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public int UnitInStock { get; set; }

    [Required]
    public int Discount { get; set; }
}
