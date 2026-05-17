namespace GameStore.Models.Dtos;

public class GenreUpdateDto
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public Guid? ParentGenreId { get; set; }
}
