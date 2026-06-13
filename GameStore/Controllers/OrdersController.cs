using GameStore.Models.Dtos;
using GameStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;
    private readonly Guid _customerId = Guid.Parse("5aa1c97e-e6b3-497c-8e00-270e96aa0b63");

    [HttpPost]
    [Route("/games/{key}/buy")]
    public async Task<IActionResult> AddGameToCart(string key)
    {
        var result = await _orderService.AddGameToCartAsync(_customerId, key);
        return result.IsSuccess
            ? StatusCode(result.StatusCode)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpDelete]
    [Route("cart/{key}")]
    public async Task<IActionResult> DeleteGameFromCart(string key)
    {
        var result = await _orderService.DeleteGameFromCartAsync(_customerId, key);
        return result.IsSuccess
            ? StatusCode(result.StatusCode)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet("cart")]
    public async Task<IActionResult> GetCart()
    {
        var result = await _orderService.GetCartAsync(_customerId);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var result = await _orderService.GetPaidAndCancelledOrdersAsync(_customerId);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var result = await _orderService.GetOrderByIdAsync(id);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet("{id:guid}/details")]
    public async Task<IActionResult> GetOrderDetails(Guid id)
    {
        var result = await _orderService.GetOrderDetailsAsync(id);
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpGet("payment-methods")]
    public async Task<IActionResult> GetPaymentMethods()
    {
        var result = await _orderService.GetPaymentMethodsAsync();
        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    [HttpPost("payment")]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestDto? request)
    {
        if (request is null)
        {
            return BadRequest("Payment request is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Method))
        {
            return BadRequest("Payment method is required.");
        }

        return request.Method.ToLowerInvariant() switch
        {
            "ibox terminal" => await ProcessIBoxPayment(),
            "visa" => await ProcessVisaPayment(request),
            "bank" => await ProcessBankPayment(request),
            _ => await ProcessDefaultPayment(request),
        };
    }

    private async Task<IActionResult> ProcessIBoxPayment()
    {
        var result = await _orderService.ProcessIBoxPaymentAsync(_customerId);

        return result.IsSuccess
            ? StatusCode(result.StatusCode, result.Value)
            : StatusCode(result.StatusCode, result.Error);
    }

    private async Task<IActionResult> ProcessVisaPayment(PaymentRequestDto request)
    {
        if (request.Model == null)
        {
            return BadRequest("Card details are required.");
        }

        var result = await _orderService.ProcessVisaPaymentAsync(_customerId, request.Model);

        return result.IsSuccess
            ? StatusCode(result.StatusCode)
            : StatusCode(result.StatusCode, result.Error);
    }

    private async Task<IActionResult> ProcessDefaultPayment(PaymentRequestDto request)
    {
        var result = await _orderService.ProcessPaymentAsync(_customerId, request);

        return result.IsSuccess
            ? StatusCode(result.StatusCode)
            : StatusCode(result.StatusCode, result.Error);
    }

    private async Task<IActionResult> ProcessBankPayment(PaymentRequestDto request)
    {
        var result = await _orderService.ProcessPaymentAsync(_customerId, request);

        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, result.Error);
        }

        var order = result.Value;
        if (order is null)
        {
            return Ok();
        }

        var invoiceResult = await _orderService.GenerateBankPaymentInvoiceAsync(_customerId, order);

        if (!invoiceResult.IsSuccess || invoiceResult.Value.Content.Length == 0)
        {
            return Ok();
        }

        return File(invoiceResult.Value.Content, "application/pdf", invoiceResult.Value.FileName);
    }
}
