using AccountingSystem.WEBAPI.Entities.Base;

namespace AccountingSystem.WEBAPI.Entities;

public sealed class Product : BaseEntity
{
    public string Name { get; set; } = default!;
    public decimal UnitPrice { get; set; } = default!;
    public string Unit { get; set; } = default!; // Adet, Kg, Litre
    public bool IsActive { get; set; } = true;

    public List<InvoiceItem> InvoiceItems { get; set; } = new();
}
