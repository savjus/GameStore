namespace GameStore.Models.Dtos;

public class VisaCardDetailsDto
{
    public string Holder { get; set; } = string.Empty;

    public string CardNumber { get; set; } = string.Empty;

    public int MonthExpire { get; set; }

    public int YearExpire { get; set; }

    public int Cvv2 { get; set; }
}
