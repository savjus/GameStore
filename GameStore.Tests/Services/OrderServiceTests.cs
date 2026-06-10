using GameStore.Data;
using GameStore.Models;
using GameStore.Models.Dtos;
using GameStore.Repositories;
using GameStore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameStore.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IOrderRepository> _orders = new();
    private readonly Mock<IGameRepository> _games = new();
    private readonly Mock<IConfiguration> _config = new();
    private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
    private readonly Mock<ILogger<OrderService>> _logger = new();

    [Fact]
    public async Task AddGameToCart_InvalidKey_Returns400()
    {
        var service = CreateService();

        var result = await service.AddGameToCartAsync(Guid.NewGuid(), string.Empty);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task AddGameToCart_GameNotFound_Returns404()
    {
        _games.Setup(x => x.GetByKeyTrackingAsync("abc"))
            .ReturnsAsync((Game)null);

        var service = CreateService();

        var result = await service.AddGameToCartAsync(Guid.NewGuid(), "abc");

        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task AddGameToCart_OutOfStock_Returns400()
    {
        _games.Setup(x => x.GetByKeyTrackingAsync("abc"))
            .ReturnsAsync(new Game { Name = "rand", Key = "ra", Id = Guid.NewGuid(), UnitInStock = 0 });

        var service = CreateService();

        var result = await service.AddGameToCartAsync(Guid.NewGuid(), "abc");

        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task AddGameToCart_Success_Returns201()
    {
        var game = new Game
        {
            Name = "rand",
            Key = "ra",
            Id = Guid.NewGuid(),
            UnitInStock = 5,
            Price = 10,
            Discount = 0,
        };

        _games.Setup(x => x.GetByKeyTrackingAsync("abc"))
            .ReturnsAsync(game);

        _orders.Setup(x => x.GetOpenOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Order { Date = DateTime.Today, CustomerId = Guid.NewGuid(), Status = OrderStatus.Open, Id = Guid.NewGuid() });

        _orders.Setup(x => x.GetOrderGameAsync(It.IsAny<Guid>(), game.Id))
            .ReturnsAsync((OrderGame)null);

        var service = CreateService();

        var result = await service.AddGameToCartAsync(Guid.NewGuid(), "abc");

        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.True(game.UnitInStock <= 4);
    }

    [Fact]
    public async Task DeleteGameFromCart_NoCart_Returns404()
    {
        _games.Setup(x => x.GetByKeyTrackingAsync("abc"))
            .ReturnsAsync(new Game() { Name = "rand", Key = "ra" });

        _orders.Setup(x => x.GetOpenOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Order)null);

        var service = CreateService();

        var result = await service.DeleteGameFromCartAsync(Guid.NewGuid(), "abc");

        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task DeleteGameFromCart_GameNotInCart_Returns404()
    {
        var game = new Game { Name = "rand", Key = "ra", Id = Guid.NewGuid() };
        var order = new Order { Date = DateTime.Today, CustomerId = Guid.NewGuid(), Status = OrderStatus.Open, Id = Guid.NewGuid() };

        _games.Setup(x => x.GetByKeyTrackingAsync("abc"))
            .ReturnsAsync(game);

        _orders.Setup(x => x.GetOpenOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync(order);

        _orders.Setup(x => x.GetOrderGameAsync(order.Id, game.Id))
            .ReturnsAsync((OrderGame)null);

        var service = CreateService();

        var result = await service.DeleteGameFromCartAsync(Guid.NewGuid(), "abc");

        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task ProcessVisa_InvalidCard_Returns400()
    {
        var service = CreateService();

        var result = await service.ProcessVisaPaymentAsync(
            Guid.NewGuid(),
            new VisaCardDetailsDto
            {
                CardNumber = "123",
                Holder = string.Empty,
                MonthExpire = 1,
                YearExpire = 2000,
                Cvv2 = 1,
            });

        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task GetPaymentMethods_ReturnsMethods()
    {
        var service = CreateService();

        var result = await service.GetPaymentMethodsAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.PaymentMethods.Count);
    }

    [Fact]
    public async Task GetCart_EmptyCart_ReturnsEmptyList()
    {
        _orders.Setup(x => x.GetOpenOrderAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Order)null);

        var service = CreateService();

        var result = await service.GetCartAsync(Guid.NewGuid());

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }

    private OrderService CreateService(HttpClient? client = null)
    {
        _uow.SetupGet(x => x.Orders).Returns(_orders.Object);
        _uow.SetupGet(x => x.Games).Returns(_games.Object);

        if (client != null)
        {
            _httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(client);
        }

        return new OrderService(
            _uow.Object,
            _config.Object,
            _httpClientFactory.Object,
            _logger.Object);
    }
}