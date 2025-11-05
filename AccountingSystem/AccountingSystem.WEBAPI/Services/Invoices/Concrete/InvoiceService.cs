using AccountingSystem.WEBAPI.Context;
using AccountingSystem.WEBAPI.DTOs.Invoices;
using AccountingSystem.WEBAPI.Entities;
using AccountingSystem.WEBAPI.Services.Invoices.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AccountingSystem.WEBAPI.Services.Invoices.Concrete;

public class InvoiceService : IInvoiceService
{
    private readonly ApplicationDbContext _context;

    public InvoiceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InvoiceDetailDto?> GetByIdAsync(int id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.InvoiceItems)
            .ThenInclude(ii => ii.Product)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
        {
            return null;
        }

        return new InvoiceDetailDto
        {
            Id = invoice.Id,
            CustomerName = $"{invoice.Customer!.Name} {invoice.Customer.Surname}",
            CreatedDate = invoice.CreatedDate,
            DueDate = invoice.DueDate,
            TotalAmount = invoice.TotalAmount,
            PaidAmount = invoice.PaidAmount,
            Items = invoice.InvoiceItems.Select(ii => new InvoiceItemDto
            {
                ProductId = ii.ProductId,
                Quantity = ii.Quantity,
                UnitPrice = ii.UnitPrice
            }).ToList()
        };
    }

    public async Task<List<InvoiceListDto>> GetAllAsync()
    {
        var invoices = await _context.Invoices
            .Include(i => i.Customer)
            .AsNoTracking()
            .ToListAsync();

        return invoices.Select(i => new InvoiceListDto
        {
            Id = i.Id,
            CustomerName = $"{i.Customer!.Name} {i.Customer!.Surname}",
            CreatedDate = i.CreatedDate,
            DueDate = i.DueDate,
            TotalAmount = i.TotalAmount,
            PaidAmount = i.PaidAmount
        }).ToList();
    }

    public async Task<InvoiceDetailDto> CreateAsync(CreateInvoiceDto request)
    {
        // Fatura
        var invoice = new Invoice
        {
            CustomerId = request.CustomerId,
            CreatedDate = DateTime.UtcNow,
            DueDate = request.DueDate,
            TotalAmount = request.Items.Sum(x => x.Quantity * x.UnitPrice),
            PaidAmount = 0
        };

        // Fatura kalemleri
        foreach (var item in request.Items)
        {
            invoice.InvoiceItems.Add(new InvoiceItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            });
        }

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        return new InvoiceDetailDto
        {
            Id = invoice.Id,
            CustomerName = (await _context.Customers.FindAsync(invoice.CustomerId))!.Name,
            CreatedDate = invoice.CreatedDate,
            DueDate = invoice.DueDate,
            TotalAmount = invoice.TotalAmount,
            PaidAmount = invoice.PaidAmount,
            Items = invoice.InvoiceItems.Select(ii => new InvoiceItemDto
            {
                ProductId = ii.ProductId,
                Quantity = ii.Quantity,
                UnitPrice = ii.UnitPrice
            }).ToList()
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null)
        {
            return false;
        }

        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync();
        return true;
    }
}
