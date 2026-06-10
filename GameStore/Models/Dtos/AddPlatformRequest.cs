using System.ComponentModel.DataAnnotations;

namespace GameStore.Models.Dtos;

public class AddPlatformRequest
{
    [Required]
    public required PlatformCreateDto Platform { get; set; }
}
