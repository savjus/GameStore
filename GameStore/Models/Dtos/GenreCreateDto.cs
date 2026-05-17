namespace GameStore.Models.Dtos;

public class GenreCreateDto
{
    public required string Name { get; set; }

    public Guid? ParentGenreId { get; set; }
}
