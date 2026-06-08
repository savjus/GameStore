using System.ComponentModel.DataAnnotations;

namespace GameStore.Models.Dtos;

public class AddGenreRequest
{
    [Required]
    public required GenreCreateDto Genre { get; set; }
}
