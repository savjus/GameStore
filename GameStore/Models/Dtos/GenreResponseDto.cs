namespace GameStore.Models.Dtos;

public class GenreResponseDto
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public Guid? ParentGenreId { get; set; }
}
