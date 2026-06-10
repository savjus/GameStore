using GameStore.Models;
using GameStore.Models.Dtos;

namespace GameStore.Services;

public interface IOrderService
{
    Task<ServiceResult> AddGameToCartAsync(Guid customerId, string gameKey);

    Task<ServiceResult> DeleteGameFromCartAsync(Guid customerId, string gameKey);

    Task<ServiceResult<List<OrderGameDto>>> GetCartAsync(Guid customerId);

    Task<ServiceResult<List<OrderGameDto>>> GetOrderDetailsAsync(Guid orderId);

    Task<ServiceResult<List<OrderDto>>> GetPaidAndCancelledOrdersAsync(Guid customerId);

    Task<ServiceResult<OrderDto>> GetOrderByIdAsync(Guid orderId);

    Task<ServiceResult<PaymentMethodsResponseDto>> GetPaymentMethodsAsync();

    Task<ServiceResult> ProcessPaymentAsync(Guid customerId, PaymentRequestDto request);

    Task<ServiceResult<(byte[] Content, string FileName)>> GenerateBankPaymentInvoiceAsync(Guid customerId, Order order);

    Task<ServiceResult<IBoxPaymentResponseDto?>> ProcessIBoxPaymentAsync(Guid customerId);

    Task<ServiceResult> ProcessVisaPaymentAsync(Guid customerId, VisaCardDetailsDto cardDetails);
}
