using System.ComponentModel.DataAnnotations;

namespace GameStore.Models.Dtos;

public class AddGameRequest
{
    [Required]
    public required GameCreateDto Game { get; set; }

    public List<Guid> Genres { get; set; } = [];

    public List<Guid> Platforms { get; set; } = [];

    [Required]
    public Guid Publisher { get; set; }
}
