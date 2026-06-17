using System.ComponentModel.DataAnnotations;

namespace GameStore.Models.Dtos;

public class GenreUpdateDto
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    public Guid? ParentGenreId { get; set; }
}
