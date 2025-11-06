namespace AccountingSystem.WEBAPI.DTOs.Invoices;

public sealed class CreateInvoiceItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
