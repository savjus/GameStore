using System.ComponentModel.DataAnnotations;

namespace GameStore.Models;

public class CommentResponseDto
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Body { get; set; }

    public List<CommentResponseDto> ChildComments { get; set; } = new();
}