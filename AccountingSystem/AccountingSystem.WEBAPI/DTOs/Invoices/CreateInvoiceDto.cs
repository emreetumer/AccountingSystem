namespace AccountingSystem.WEBAPI.DTOs.Invoices;

public sealed class CreateInvoiceDto
{
    public int CustomerId { get; set; }
    public DateTime DueDate { get; set; }
    public List<InvoiceItemDto> Items { get; set; } = new();
}
