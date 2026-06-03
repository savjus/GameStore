using System.ComponentModel.DataAnnotations;

namespace GameStore.Models.Dtos;

public class PublisherCreateDto
{
    [Required]
    [MaxLength(200)]
    public required string CompanyName { get; set; }

    [Required]
    [MaxLength(500)]
    [Url]
    public required string HomePage { get; set; }

    [Required]
    [MaxLength(2000)]
    public required string Description { get; set; }
}
