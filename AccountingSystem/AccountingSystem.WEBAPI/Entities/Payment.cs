using AccountingSystem.WEBAPI.Entities.Base;

namespace AccountingSystem.WEBAPI.Entities;

public sealed class Payment : BaseEntity
{
    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    public decimal Amount { get; set; } // Tutar
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow; // Ödeme tarihi
    public string? Description { get; set; }
}
