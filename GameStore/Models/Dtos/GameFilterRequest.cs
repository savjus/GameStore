namespace GameStore.Models.Dtos;

public class GameFilterRequest
{
    public List<Guid> GenreIds { get; set; } = [];

    public List<Guid> PlatformIds { get; set; } = [];

    public List<Guid> PublisherIds { get; set; } = [];

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public string? PublishDateFilter { get; set; }

    public string? Name { get; set; }

    public string? SortBy { get; set; }

    public string PageSize { get; set; } = "10";

    public int Page { get; set; } = 1;
}
