namespace AccountingSystem.WEBAPI.DTOs.Payments;

public sealed class PaymentListDto
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public string CustomerName { get; set; } = default!;
    public decimal Amount { get; set; }
    public decimal KalanAmount { get; set; }



    public DateTime PaymentDate { get; set; }
    public string? Description { get; set; }
}
