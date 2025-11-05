namespace AccountingSystem.WEBAPI.DTOs.Payments;

public sealed class CreatePaymentDto
{
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}
