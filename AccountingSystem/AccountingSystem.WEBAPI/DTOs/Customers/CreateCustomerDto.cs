namespace AccountingSystem.WEBAPI.DTOs.Customers;

public sealed class CreateCustomerDto
{
    public string Name { get; set; } = default!;
    public string Surname { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
}
