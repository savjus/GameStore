using System.ComponentModel.DataAnnotations;

namespace GameStore.Models.Dtos;

public class PlatformCreateDto
{
    [Required]
    [MaxLength(100)]
    public required string Type { get; set; }
}
