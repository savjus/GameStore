using System.ComponentModel.DataAnnotations;

namespace GameStore.Models.Dtos;

public class UpdatePlatformRequest
{
    [Required]
    public required PlatformUpdateDto Platform { get; set; }
}
