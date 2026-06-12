namespace GameStore.Models.Dtos;

public class OrderGameDto
{
    public Guid ProductId { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int Discount { get; set; }

    public string ProductKey { get; set; } = string.Empty;
}
