namespace GameStore.Models;

public class Ban
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string UserName { get; set; }

    public DateTime BannedUntil { get; set; }

    public bool IsPermanent { get; set; }
}