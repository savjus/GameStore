using System.ComponentModel.DataAnnotations;

namespace GameStore.Models.Dtos;

public class UpdatePublisherRequest
{
    [Required]
    public required PublisherUpdateDto Publisher { get; set; }
}
