using System.ComponentModel.DataAnnotations;

namespace GameStore.Models;

public class CommentBodyDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;
}