// CommentRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace GameStore.Models;

public class CommentRequestDto
{
    [Required]
    public CommentBodyDto Comment { get; set; }

    public Guid? ParentId { get; set; }

    public string? Action { get; set; }
}