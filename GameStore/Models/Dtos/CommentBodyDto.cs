using System.ComponentModel.DataAnnotations;

namespace GameStore.Models;

public class CommentBodyDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Body { get; set; }
}