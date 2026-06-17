using System.ComponentModel.DataAnnotations;

namespace GameStore.Models.Dtos;

public class AddPublisherRequest
{
    [Required]
    public required PublisherCreateDto Publisher { get; set; }
}
