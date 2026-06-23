namespace GameStore.Models.Dtos;

public class PagedGamesResponseDto
{
    public List<GameResponseDto> Games { get; set; } = [];

    public int TotalPages { get; set; }

    public int CurrentPage { get; set; }
}
