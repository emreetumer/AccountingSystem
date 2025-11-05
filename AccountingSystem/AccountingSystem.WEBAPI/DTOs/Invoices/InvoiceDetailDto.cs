namespace AccountingSystem.WEBAPI.DTOs.Invoices;

public sealed class InvoiceDetailDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = default!;
    public DateTime CreatedDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }

    public List<InvoiceItemDto> Items { get; set; } = new();
}
