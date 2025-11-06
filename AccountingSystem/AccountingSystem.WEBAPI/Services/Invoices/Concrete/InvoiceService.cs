using AccountingSystem.WEBAPI.Context;
using AccountingSystem.WEBAPI.DTOs.Invoices;
using AccountingSystem.WEBAPI.Entities;
using AccountingSystem.WEBAPI.Hubs;
using AccountingSystem.WEBAPI.Services.Invoices.Abstract;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AccountingSystem.WEBAPI.Services.Invoices.Concrete;

public class InvoiceService : IInvoiceService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificationHub> _hub;

    public InvoiceService(ApplicationDbContext context, IHubContext<NotificationHub> hub)
    {
        _context = context;
        _hub = hub;
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
        var customer = await _context.Customers.FindAsync(request.CustomerId);
        if (customer == null)
        {
            throw new InvalidOperationException($"CustomerId {request.CustomerId} bulunamadı.");
        }

        var productIds = request.Items.Select(x => x.ProductId).Distinct().ToList();

        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.UnitPrice);


        var missingProducts = productIds.Except(products.Keys).ToList();
        if (missingProducts.Any())
        {
            throw new InvalidOperationException($"Şu ProductId değerleri bulunamadı: {string.Join(", ", missingProducts)}");
        }

        var totalAmount = request.Items.Sum(i => i.Quantity * products[i.ProductId]);

        var invoice = new Invoice
        {
            CustomerId = request.CustomerId,
            CreatedDate = DateTime.UtcNow,
            DueDate = request.DueDate,
            TotalAmount = totalAmount,
            PaidAmount = 0
        };

        foreach (var item in request.Items)
        {
            var unitPrice = products[item.ProductId];

            invoice.InvoiceItems.Add(new InvoiceItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = unitPrice
            });
        }


        customer.CurrentDebt += totalAmount;

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        #region SignalR
        await _hub.Clients.Group("dashboard").SendAsync("InvoiceCreated", new
        {
            invoiceId = invoice.Id,
            customerId = customer.Id,
            customerName = $"{customer.Name} {customer.Surname}",
            totalAmount = invoice.TotalAmount,
            kalanAmount = customer.CurrentDebt,
            createdDate = invoice.CreatedDate,
            dueDate = invoice.DueDate
        });

        await _hub.Clients.Group($"customer-{customer.Id}").SendAsync("CustomerDebtUpdated", new
        {
            customerId = customer.Id,
            currentDebt = customer.CurrentDebt
        });
        #endregion

        return new InvoiceDetailDto
        {
            Id = invoice.Id,
            CustomerName = $"{customer.Name} {customer.Surname}",
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
