using AccountingSystem.WEBAPI.Entities.Base;

namespace AccountingSystem.WEBAPI.Entities;

public sealed class InvoiceItem : BaseEntity
{
    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int Quantity { get; set; } // Miktar, adet
    public decimal UnitPrice { get; set; } // Birim fiyatı
}
