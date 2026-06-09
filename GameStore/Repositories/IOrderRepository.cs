using GameStore.Models;

namespace GameStore.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetOpenOrderAsync(Guid customerId);

    Task<Order?> GetCheckoutOrderAsync(Guid customerId);

    Task<Order?> GetByIdAsync(Guid orderId);

    Task<OrderGame?> GetOrderGameAsync(Guid orderId, Guid productId);

    Task<List<OrderGame>> GetOrderDetailsAsync(Guid orderId);

    Task<List<Order>> GetPaidAndCancelledOrdersAsync(Guid customerId);

    Task AddOrderAsync(Order order);

    Task AddOrderGameAsync(OrderGame orderGame);

    Task DeleteOrderGame(OrderGame orderGame);

    Task DeleteOrder(Order order);
}