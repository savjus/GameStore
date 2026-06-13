using System.ComponentModel.DataAnnotations;

namespace GameStore.Models.Dtos;

public class GenreCreateDto
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    public Guid? ParentGenreId { get; set; }
}
