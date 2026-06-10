using GameStore.Data;
using GameStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Repositories;

public class OrderRepository(GameStoreDbContext dbContext) : IOrderRepository
{
    private readonly GameStoreDbContext _dbContext = dbContext;

    public async Task AddOrderAsync(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
    }

    public async Task AddOrderGameAsync(OrderGame orderGame)
    {
        await _dbContext.GameOrders.AddAsync(orderGame);
    }

    public Task DeleteOrder(Order order)
    {
        _dbContext.Orders.Remove(order);
        return Task.CompletedTask;
    }

    public Task DeleteOrderGame(OrderGame orderGame)
    {
        _dbContext.GameOrders.Remove(orderGame);
        return Task.CompletedTask;
    }

    public async Task<Order?> GetByIdAsync(Guid orderId)
    {
        return await _dbContext.Orders
            .Include(o => o.OrderGames)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<Order?> GetCheckoutOrderAsync(Guid customerId)
    {
        return await _dbContext.Orders
            .Include(o => o.OrderGames)
            .FirstOrDefaultAsync(o => o.CustomerId == customerId && o.Status == OrderStatus.Checkout);
    }

    public async Task<Order?> GetOpenOrderAsync(Guid customerId)
    {
        return await _dbContext.Orders
            .Include(o => o.OrderGames)
            .FirstOrDefaultAsync(o => o.CustomerId == customerId && o.Status == OrderStatus.Open);
    }

    public async Task<List<OrderGame>> GetOrderDetailsAsync(Guid orderId)
    {
        return await _dbContext.GameOrders
            .Where(go => go.OrderId == orderId)
            .ToListAsync();
    }

    public Task<OrderGame?> GetOrderGameAsync(Guid orderId, Guid productId)
    {
        return _dbContext.GameOrders
            .FirstOrDefaultAsync(og => og.OrderId == orderId && og.ProductId == productId);
    }

    public Task<List<Order>> GetPaidAndCancelledOrdersAsync(Guid customerId)
    {
        return _dbContext.Orders
            .Where(o => o.CustomerId == customerId &&
                       (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Cancelled))
            .ToListAsync();
    }
}