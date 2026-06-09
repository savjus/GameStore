namespace GameStore.Models.Dtos;

public class PaymentRequestDto
{
    public string Method { get; set; } = string.Empty;

    public VisaCardDetailsDto? Model { get; set; }
}
