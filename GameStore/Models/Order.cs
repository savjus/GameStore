namespace GameStore.Models;

public enum OrderStatus
{
    Open,
    Checkout,
    Paid,
    Cancelled,
}

public class Order
{
    public required Guid Id { get; set; }

    public required DateTime Date { get; set; }

    public required Guid CustomerId { get; set; }

    public required OrderStatus Status { get; set; }

    public ICollection<OrderGame> OrderGames { get; set; } = [];
}