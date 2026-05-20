using System.ComponentModel.DataAnnotations;

namespace GameStore.Models.Dtos;

public class UpdateGenreRequest
{
    [Required]
    public required GenreUpdateDto Genre { get; set; }
}
