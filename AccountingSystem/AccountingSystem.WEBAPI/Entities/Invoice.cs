using AccountingSystem.WEBAPI.Entities.Base;

namespace AccountingSystem.WEBAPI.Entities;

public sealed class Invoice : BaseEntity
{
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }

    public List<InvoiceItem> InvoiceItems { get; set; } = new();
    public List<Payment> Payments { get; set; } = new();
}
