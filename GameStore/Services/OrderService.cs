using System.Text.Json;
using GameStore.Data;
using GameStore.Models;
using GameStore.Models.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GameStore.Services;

public class OrderService(IUnitOfWork unitOfWork,
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory,
    ILogger<OrderService> logger) : IOrderService
{
    private const int DefaultPaymentRetryCount = 3;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IConfiguration _configuration = configuration;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<OrderService> _logger = logger;

    public async Task<ServiceResult> AddGameToCartAsync(Guid customerId, string gameKey)
    {
        if (string.IsNullOrWhiteSpace(gameKey))
        {
            return ServiceResult.Fail(StatusCodes.Status400BadRequest, "Game key is required.");
        }

        var game = await _unitOfWork.Games.GetByKeyAsync(gameKey);
        if (game == null)
        {
            return ServiceResult.Fail(StatusCodes.Status404NotFound, "Game not found.");
        }

        if (game.UnitInStock <= 0)
        {
            return ServiceResult.Fail(StatusCodes.Status400BadRequest, "Game is out of stock.");
        }

        var cart = await _unitOfWork.Orders.GetOpenOrderAsync(customerId);

        if (cart == null)
        {
            cart = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Date = DateTime.UtcNow,
                Status = OrderStatus.Open,
            };
            await _unitOfWork.Orders.AddOrderAsync(cart);
            await _unitOfWork.SaveChangesAsync();
        }

        var existingOrderGame = await _unitOfWork.Orders.GetOrderGameAsync(cart.Id, game.Id);

        if (existingOrderGame != null)
        {
            if (existingOrderGame.Quantity + 1 > game.UnitInStock)
            {
                return ServiceResult.Fail(
                    StatusCodes.Status400BadRequest,
                    "Cannot add more games than available in stock.");
            }

            existingOrderGame.Quantity++;
        }
        else
        {
            var orderGame = new OrderGame
            {
                OrderId = cart.Id,
                ProductId = game.Id,
                Price = game.Price,
                Quantity = 1,
                Discount = game.Discount,
            };
            await _unitOfWork.Orders.AddOrderGameAsync(orderGame);
        }

        game.UnitInStock--;

        await _unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(StatusCodes.Status201Created);
    }

    public async Task<ServiceResult> DeleteGameFromCartAsync(Guid customerId, string gameKey)
    {
        if (string.IsNullOrWhiteSpace(gameKey))
        {
            return ServiceResult.Fail(StatusCodes.Status400BadRequest, "Game key is required.");
        }

        var game = await _unitOfWork.Games.GetByKeyAsync(gameKey);
        if (game == null)
        {
            return ServiceResult.Fail(StatusCodes.Status404NotFound, "Game not found.");
        }

        var cart = await _unitOfWork.Orders.GetOpenOrderAsync(customerId);
        if (cart == null)
        {
            return ServiceResult.Fail(StatusCodes.Status404NotFound, "Cart not found.");
        }

        var orderGame = await _unitOfWork.Orders.GetOrderGameAsync(cart.Id, game.Id);
        if (orderGame == null)
        {
            return ServiceResult.Fail(
                StatusCodes.Status404NotFound,
                "Game not found in cart.");
        }

        game.UnitInStock += orderGame.Quantity;

        await _unitOfWork.Orders.DeleteOrderGame(orderGame);

        var remainingItems = await _unitOfWork.Orders.GetOrderDetailsAsync(cart.Id);
        if (remainingItems.Count == 0)
        {
            await _unitOfWork.Orders.DeleteOrder(cart);
        }

        await _unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(StatusCodes.Status204NoContent);
    }

    public async Task<ServiceResult<List<OrderGameDto>>> GetCartAsync(Guid customerId)
    {
        var cart = await _unitOfWork.Orders.GetOpenOrderAsync(customerId);
        if (cart == null)
        {
            return ServiceResult.Success(new List<OrderGameDto>(), StatusCodes.Status200OK);
        }

        var orderGames = await _unitOfWork.Orders.GetOrderDetailsAsync(cart.Id);
        var dtos = orderGames.Select(og => new OrderGameDto
        {
            ProductId = og.ProductId,
            Price = og.Price,
            Quantity = og.Quantity,
            Discount = og.Discount,
        }).ToList();

        return ServiceResult.Success(dtos, StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<List<OrderGameDto>>> GetOrderDetailsAsync(Guid orderId)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
        if (order == null)
        {
            return ServiceResult.Fail<List<OrderGameDto>>(
                StatusCodes.Status404NotFound,
                "Order not found.");
        }

        var orderGames = await _unitOfWork.Orders.GetOrderDetailsAsync(orderId);
        var dtos = orderGames.Select(og => new OrderGameDto
        {
            ProductId = og.ProductId,
            Price = og.Price,
            Quantity = og.Quantity,
            Discount = og.Discount,
        }).ToList();

        return ServiceResult.Success(dtos, StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<List<OrderDto>>> GetPaidAndCancelledOrdersAsync(Guid customerId)
    {
        var orders = await _unitOfWork.Orders.GetPaidAndCancelledOrdersAsync(customerId);
        var dtos = orders.Select(o => new OrderDto
        {
            Id = o.Id,
            CustomerId = o.CustomerId,
            Date = o.Date,
        }).ToList();

        return ServiceResult.Success(dtos, StatusCodes.Status200OK);
    }

    public async Task<ServiceResult<OrderDto>> GetOrderByIdAsync(Guid orderId)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
        if (order == null)
        {
            return ServiceResult.Fail<OrderDto>(
                StatusCodes.Status404NotFound,
                "Order not found.");
        }

        var dto = new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Date = order.Date,
        };

        return ServiceResult.Success(dto, StatusCodes.Status200OK);
    }

    public Task<ServiceResult<PaymentMethodsResponseDto>> GetPaymentMethodsAsync()
    {
        var paymentMethods = new PaymentMethodsResponseDto
        {
            PaymentMethods = new List<PaymentMethodDto>
            {
                new PaymentMethodDto
                {
                    ImageUrl = "https://via.placeholder.com/150?text=Bank",
                    Title = "Bank",
                    Description = "Pay using bank transfer with invoice",
                },
                new PaymentMethodDto
                {
                    ImageUrl = "https://via.placeholder.com/150?text=IBox",
                    Title = "IBox terminal",
                    Description = "Pay using IBox terminal",
                },
                new PaymentMethodDto
                {
                    ImageUrl = "https://via.placeholder.com/150?text=Visa",
                    Title = "Visa",
                    Description = "Pay using Visa card",
                },
            },
        };

        return Task.FromResult(ServiceResult.Success(paymentMethods, StatusCodes.Status200OK));
    }

    public async Task<ServiceResult> ProcessPaymentAsync(Guid customerId, PaymentRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Method))
        {
            return ServiceResult.Fail(StatusCodes.Status400BadRequest, "Payment method is required.");
        }

        var order = await GetOrderForPaymentAsync(customerId);
        if (order == null)
        {
            return ServiceResult.Fail(StatusCodes.Status404NotFound, "No open order found.");
        }

        if (order.Status == OrderStatus.Open)
        {
            order.Status = OrderStatus.Checkout;
            await _unitOfWork.SaveChangesAsync();
        }

        if (request.Method.Equals("Bank", StringComparison.OrdinalIgnoreCase))
        {
            return await ProcessBankPaymentAsync(customerId, order);
        }
        else if (request.Method.Equals("IBox terminal", StringComparison.OrdinalIgnoreCase))
        {
            var result = await ProcessIBoxPaymentInternalAsync(customerId, order);
            return result.IsSuccess
                ? ServiceResult.Success(result.StatusCode)
                : ServiceResult.Fail(result.StatusCode, result.Error ?? "IBox payment failed.");
        }
        else if (request.Method.Equals("Visa", StringComparison.OrdinalIgnoreCase))
        {
            return await ProcessVisaPaymentInternalAsync(customerId, order, request.Model!);
        }
        else
        {
            return ServiceResult.Fail(StatusCodes.Status400BadRequest, "Invalid payment method.");
        }
    }

    public async Task<ServiceResult<IBoxPaymentResponseDto?>> ProcessIBoxPaymentAsync(Guid customerId)
    {
        var order = await GetOrderForPaymentAsync(customerId);
        if (order == null)
        {
            return ServiceResult.Fail<IBoxPaymentResponseDto?>(
                StatusCodes.Status404NotFound,
                "No open order found.");
        }

        if (order.Status == OrderStatus.Open)
        {
            order.Status = OrderStatus.Checkout;
            await _unitOfWork.SaveChangesAsync();
        }

        return await ProcessIBoxPaymentInternalAsync(customerId, order);
    }

    public async Task<ServiceResult> ProcessVisaPaymentAsync(Guid customerId, VisaCardDetailsDto cardDetails)
    {
        if (cardDetails == null)
        {
            return ServiceResult.Fail(StatusCodes.Status400BadRequest, "Card details are required.");
        }

        if (!IsValidVisaCardDetails(cardDetails))
        {
            return ServiceResult.Fail(StatusCodes.Status400BadRequest, "Card details are invalid.");
        }

        var order = await GetOrderForPaymentAsync(customerId);
        if (order == null)
        {
            return ServiceResult.Fail(StatusCodes.Status404NotFound, "No open order found.");
        }

        if (order.Status == OrderStatus.Open)
        {
            order.Status = OrderStatus.Checkout;
            await _unitOfWork.SaveChangesAsync();
        }

        return await ProcessVisaPaymentInternalAsync(customerId, order, cardDetails);
    }

    public async Task<ServiceResult<(byte[] Content, string FileName)>> GenerateBankPaymentInvoiceAsync(Guid customerId, Order order)
    {
        try
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var sum = await CalculateOrderSumAsync(order);
            var invoiceValidityDays = _configuration.GetValue<int?>("BankPayment:InvoiceValidityDays") ?? 30;
            var expiryDate = order.Date.AddDays(invoiceValidityDays);

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Content().Column(col =>
                    {
                        col.Item().AlignCenter().Text("INVOICE")
                            .Bold().FontSize(16);

                        col.Item().Height(20);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                            });

                            void AddRow(string label, string value)
                            {
                                table.Cell().Text(label).Bold();
                                table.Cell().Text(value);
                            }

                            AddRow("User ID:",        customerId.ToString());
                            AddRow("Order ID:",       order.Id.ToString());
                            AddRow("Creation Date:",  order.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                            AddRow("Validity Date:",  expiryDate.ToString("yyyy-MM-dd"));
                            AddRow("Amount:",         $"{sum:F2}");
                        });
                    });
                });
            }).GeneratePdf(); // returns byte[] directly, no stream wrangling

            var fileName = $"Invoice_{order.Id}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";
            return ServiceResult.Success((pdfBytes, fileName), StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail<(byte[] Content, string FileName)>(
                StatusCodes.Status500InternalServerError,
                $"Failed to generate invoice: {ex.Message}");
        }
    }

    private async Task<Order?> GetOrderForPaymentAsync(Guid customerId)
    {
        var openOrder = await _unitOfWork.Orders.GetOpenOrderAsync(customerId);
        if (openOrder != null)
        {
            return openOrder;
        }

        return await _unitOfWork.Orders.GetCheckoutOrderAsync(customerId);
    }

    private async Task<ServiceResult> ProcessBankPaymentAsync(Guid customerId, Order order)
    {
        try
        {
            order.Status = OrderStatus.Paid;
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Success(StatusCodes.Status200OK);
        }
        catch (Exception)
        {
            order.Status = OrderStatus.Open;
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Fail(StatusCodes.Status500InternalServerError, "Bank payment failed.");
        }
    }

    private async Task<ServiceResult<IBoxPaymentResponseDto?>> ProcessIBoxPaymentInternalAsync(
        Guid customerId, Order order)
    {
        var microserviceUrl = _configuration["PaymentMicroservice:IBoxUrl"];
        if (string.IsNullOrWhiteSpace(microserviceUrl))
        {
            order.Status = OrderStatus.Open;
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Fail<IBoxPaymentResponseDto?>(
                StatusCodes.Status500InternalServerError,
                "IBox payment URL is not configured.");
        }

        var sum = await CalculateOrderSumAsync(order);
        var retryCount = GetPaymentRetryCount();
        var httpClient = _httpClientFactory.CreateClient();
        string? lastError = null;

        for (var attempt = 1; attempt <= retryCount; attempt++)
        {
            try
            {
                var payload = new
                {
                    userId = customerId,
                    orderId = order.Id,
                    sum,
                };

                using var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    System.Text.Encoding.UTF8,
                    "application/json");

                _logger.LogInformation("IBox request to {Url} payload: {Payload}", microserviceUrl, JsonSerializer.Serialize(payload));
                using var response = await httpClient.PostAsync(microserviceUrl, content);
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("IBox response {Status} {Reason} body: {Body}", (int)response.StatusCode, response.ReasonPhrase, responseBody);
                if (!response.IsSuccessStatusCode)
                {
                    var responseReason = string.IsNullOrWhiteSpace(response.ReasonPhrase)
                        ? "Unknown"
                        : response.ReasonPhrase;
                    lastError =
                        $"IBox payment request failed with status code {(int)response.StatusCode} ({responseReason})."
                        + (string.IsNullOrWhiteSpace(responseBody)
                            ? string.Empty
                            : $" Body: {responseBody}");
                    continue;
                }

                var parseResult = TryValidateAndParseIBoxResponse(responseBody, customerId, order.Id, sum);
                if (!parseResult.IsValid)
                {
                    lastError = parseResult.Error;
                    continue;
                }

                order.Status = OrderStatus.Paid;
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult.Success(parseResult.Response, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                lastError = $"IBox payment attempt {attempt} failed: {ex.Message}";
            }
        }

        order.Status = OrderStatus.Open;
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult.Fail<IBoxPaymentResponseDto?>(
            StatusCodes.Status502BadGateway,
            lastError ?? "IBox payment failed after retries.");
    }

    private async Task<ServiceResult> ProcessVisaPaymentInternalAsync(
        Guid customerId, Order order, VisaCardDetailsDto cardDetails)
    {
        if (cardDetails == null || !IsValidVisaCardDetails(cardDetails))
        {
            return ServiceResult.Fail(StatusCodes.Status400BadRequest, "Card details are invalid.");
        }

        var microserviceUrl = _configuration["PaymentMicroservice:VisaUrl"];
        if (string.IsNullOrWhiteSpace(microserviceUrl))
        {
            order.Status = OrderStatus.Open;
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Fail(StatusCodes.Status500InternalServerError, "Visa payment URL is not configured.");
        }

        var sum = await CalculateOrderSumAsync(order);
        var retryCount = GetPaymentRetryCount();
        var httpClient = _httpClientFactory.CreateClient();
        string? lastError = null;

        for (var attempt = 1; attempt <= retryCount; attempt++)
        {
            try
            {
                var payload = new
                {
                    userId = customerId,
                    orderId = order.Id,
                    sum,
                    CardHolderName = cardDetails.Holder,
                    CardNumber = cardDetails.CardNumber,
                    ExpirationMonth = cardDetails.MonthExpire,
                    ExpirationYear = cardDetails.YearExpire,
                    Cvv = cardDetails.Cvv2,
                };

                using var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    System.Text.Encoding.UTF8,
                    "application/json");

                _logger.LogInformation("Visa request to {Url} payload: {Payload}", microserviceUrl, JsonSerializer.Serialize(payload));
                using var response = await httpClient.PostAsync(microserviceUrl, content);
                var responseBodyVisa = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Visa response {Status} {Reason} body: {Body}", (int)response.StatusCode, response.ReasonPhrase, responseBodyVisa);
                if (!response.IsSuccessStatusCode)
                {
                    var responseReason = string.IsNullOrWhiteSpace(response.ReasonPhrase)
                        ? "Unknown"
                        : response.ReasonPhrase;
                    lastError =
                        $"Visa payment request failed with status code {(int)response.StatusCode} ({responseReason})."
                        + (string.IsNullOrWhiteSpace(responseBodyVisa)
                            ? string.Empty
                            : $" Body: {responseBodyVisa}");
                    continue;
                }

                if (!IsValidVisaResponse(responseBodyVisa))
                {
                    lastError = "Visa payment returned invalid response payload.";
                    continue;
                }

                order.Status = OrderStatus.Paid;
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult.Success(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                lastError = $"Visa payment attempt {attempt} failed: {ex.Message}";
            }
        }

        order.Status = OrderStatus.Open;
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult.Fail(StatusCodes.Status502BadGateway, lastError ?? "Visa payment failed after retries.");
    }

    private int GetPaymentRetryCount()
    {
        var configuredRetry = _configuration.GetValue<int?>("PaymentMicroservice:RetryCount");
        return configuredRetry is > 0 ? configuredRetry.Value : DefaultPaymentRetryCount;
    }

    private static bool IsValidVisaCardDetails(VisaCardDetailsDto cardDetails)
    {
        var hasValidHolder = !string.IsNullOrWhiteSpace(cardDetails.Holder);
        var hasValidCardNumber = !string.IsNullOrWhiteSpace(cardDetails.CardNumber)
            && cardDetails.CardNumber.All(char.IsDigit)
            && cardDetails.CardNumber.Length is >= 12 and <= 19;
        var hasValidMonth = cardDetails.MonthExpire is >= 1 and <= 12;
        var hasValidYear = cardDetails.YearExpire >= DateTime.UtcNow.Year;
        var hasValidCvv = cardDetails.Cvv2 is >= 100 and <= 9999;

        return hasValidHolder && hasValidCardNumber && hasValidMonth && hasValidYear && hasValidCvv;
    }

    private static bool IsValidVisaResponse(string responseContent)
    {
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return true;
        }

        try
        {
            _ = JsonDocument.Parse(responseContent);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static (bool IsValid, IBoxPaymentResponseDto? Response, string? Error) TryValidateAndParseIBoxResponse(
        string responseContent,
        Guid expectedCustomerId,
        Guid expectedOrderId,
        double expectedSum)
    {
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return (false, null, "IBox payment response is empty.");
        }

        try
        {
            var response = JsonSerializer.Deserialize<IBoxPaymentResponseDto>(
                responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (response == null)
            {
                return (false, null, "IBox payment response body is invalid.");
            }

            // Accept microservice variants and normalize missing fields.
            if (response.UserId == Guid.Empty)
            {
                response.UserId = expectedCustomerId;
            }

            if (response.OrderId == Guid.Empty)
            {
                response.OrderId = expectedOrderId;
            }

            if (response.PaymentDate == default)
            {
                response.PaymentDate = DateTime.UtcNow;
            }

            if (response.Sum <= 0)
            {
                response.Sum = expectedSum;
            }

            return (true, response, null);
        }
        catch (JsonException)
        {
            return (false, null, "IBox payment response JSON is malformed.");
        }
    }

    private async Task<double> CalculateOrderSumAsync(Order order)
    {
        var orderDetails = await _unitOfWork.Orders.GetOrderDetailsAsync(order.Id);
        double sum = 0;

        foreach (var item in orderDetails)
        {
            var itemPrice = item.Price * item.Quantity;
            var discount = (itemPrice * item.Discount) / 100;
            sum += itemPrice - discount;
        }

        return sum;
    }
}
