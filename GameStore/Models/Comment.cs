namespace GameStore.Models;

public class Comment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Name { get; set; } = null!;

    public required string Body { get; set; } = null!;

    public Guid? ParentCommentId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Comment? ParentComment { get; set; }

    public ICollection<Comment> ChildComments { get; set; } = [];

    public required Guid GameId { get; set; }

    public Game? Game { get; set; }
}