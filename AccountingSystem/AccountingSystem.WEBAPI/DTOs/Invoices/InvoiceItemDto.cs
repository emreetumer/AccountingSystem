namespace AccountingSystem.WEBAPI.DTOs.Invoices;

public sealed class InvoiceItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
