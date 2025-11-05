using AccountingSystem.WEBAPI.DTOs.Customers;

namespace AccountingSystem.WEBAPI.Services.Customer.Abstract;

public interface ICustomerService
{
    Task<CustomerListDto?> GetByIdAsync(int id);
    Task<List<CustomerListDto>> GetAllAsync();
    Task<CustomerListDto> CreateAsync(CreateCustomerDto request);
    Task<CustomerListDto?> UpdateAsync(UpdateCustomerDto request);
    Task<bool> DeleteAsync(int id);
}
