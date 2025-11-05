using AccountingSystem.WEBAPI.Entities.Base;

namespace AccountingSystem.WEBAPI.Entities;

public sealed class Customer : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Surname { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;

    public decimal CurrentDebt { get; set; }

    public List<Invoice> Invoices { get; set; } = new();
}
