namespace GameStore.Models.Dtos;

public class IBoxPaymentResponseDto
{
    public Guid UserId { get; set; }

    public Guid OrderId { get; set; }

    public DateTime PaymentDate { get; set; }

    public decimal Sum { get; set; }
}
