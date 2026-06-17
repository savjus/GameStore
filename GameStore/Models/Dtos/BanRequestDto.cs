using System.ComponentModel.DataAnnotations;

namespace GameStore.Models;

public class BanRequestDto
{
    [Required]
    public string User { get; set; } = null!;

    [Required]
    public string Duration { get; set; } = null!;
}