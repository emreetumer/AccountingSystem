namespace AccountingSystem.WEBAPI.DTOs.Invoices;

public sealed class InvoiceListDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = default!;
    public DateTime CreatedDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
}
