using System.ComponentModel.DataAnnotations;

namespace GameStore.Models.Dtos;

public class PlatformUpdateDto
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Type { get; set; }
}
