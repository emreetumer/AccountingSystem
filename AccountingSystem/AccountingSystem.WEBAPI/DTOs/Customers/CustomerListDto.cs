namespace AccountingSystem.WEBAPI.DTOs.Customers;

public sealed class CustomerListDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public decimal CurrentDebt { get; set; }
}
