using AccountingSystem.WEBAPI.Context;
using AccountingSystem.WEBAPI.DTOs.Customers;
using AccountingSystem.WEBAPI.Services.Customer.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AccountingSystem.WEBAPI.Services.Customer.Concrete;

public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _context;
    public CustomerService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<CustomerListDto?> GetByIdAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
        {
            return null;
        }

        return new CustomerListDto
        {
            Id = customer.Id,
            FullName = $"{customer.Name} {customer.Surname}",
            Email = customer.Email,
            Phone = customer.Phone,
            CurrentDebt = customer.CurrentDebt,
        };
    }

    public async Task<List<CustomerListDto>> GetAllAsync()
    {
        return await _context.Customers.Select(c => new CustomerListDto
        {
            Id = c.Id,
            FullName = $"{c.Name} {c.Surname}",
            Email = c.Email,
            Phone = c.Phone,
            CurrentDebt = c.CurrentDebt
        }).AsNoTracking().ToListAsync();
    }

    public async Task<CustomerListDto> CreateAsync(CreateCustomerDto request)
    {
        var entity = new Entities.Customer
        {
            Name = request.Name,
            Surname = request.Surname,
            Email = request.Email,
            Phone = request.Phone,
            CurrentDebt = 0
        };

        _context.Customers.Add(entity);
        await _context.SaveChangesAsync();

        return new CustomerListDto
        {
            Id = entity.Id,
            FullName = $"{entity.Name} {entity.Surname}",
            Email = entity.Email,
            Phone = entity.Phone,
            CurrentDebt = entity.CurrentDebt
        };
    }

    public async Task<CustomerListDto?> UpdateAsync(UpdateCustomerDto request)
    {
        var customer = await _context.Customers.FindAsync(request.Id);
        if (customer == null)
        {
            return null;
        }

        customer.Name = request.Name;
        customer.Surname = request.Surname;
        customer.Email = request.Email;
        customer.Phone = request.Phone;

        await _context.SaveChangesAsync();

        return new CustomerListDto
        {
            Id = customer.Id,
            FullName = $"{customer.Name} {customer.Surname}",
            Email = customer.Email,
            Phone = customer.Phone,
            CurrentDebt = customer.CurrentDebt
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
        {
            return false;
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return true;
    }
}
