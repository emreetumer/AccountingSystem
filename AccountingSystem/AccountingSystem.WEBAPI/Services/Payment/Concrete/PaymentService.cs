using AccountingSystem.WEBAPI.Context;
using AccountingSystem.WEBAPI.DTOs.Payments;
using AccountingSystem.WEBAPI.Services.Payment.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AccountingSystem.WEBAPI.Services.Payment.Concrete;

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;

    public PaymentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PaymentListDto>> GetAllAsync()
    {
        var payments = await _context.Payments
            .Include(p => p.Invoice)
            .ThenInclude(i => i!.Customer)
            .AsNoTracking()
            .ToListAsync();

        return payments.Select(p => new PaymentListDto
        {
            Id = p.Id,
            InvoiceId = p.InvoiceId,
            CustomerName = $"{p.Invoice!.Customer!.Name} {p.Invoice.Customer.Surname}",
            Amount = p.Amount,
            PaymentDate = p.PaymentDate,
            Description = p.Description
        }).ToList();
    }

    public async Task<PaymentListDto?> GetByIdAsync(int id)
    {
        var payment = await _context.Payments
            .Include(p => p.Invoice)
            .ThenInclude(i => i!.Customer)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null)
        {
            return null;
        }

        return new PaymentListDto
        {
            Id = payment.Id,
            InvoiceId = payment.InvoiceId,
            CustomerName = $"{payment.Invoice!.Customer!.Name} {payment.Invoice.Customer.Surname}",
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate,
            Description = payment.Description
        };
    }

    public async Task<PaymentListDto> CreateAsync(CreatePaymentDto request)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Customer)
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId);

        if (invoice == null)
        {
            throw new InvalidOperationException($"InvoiceId {request.InvoiceId} bulunamadı.");
        }

        var customer = invoice.Customer!;
        if (customer == null)
        {
            throw new InvalidOperationException($"Bu fatura için müşteri bilgisi bulunamadı.");
        }

        // Ödeme işlemi
        invoice.PaidAmount += request.Amount;

        // Müşterinin toplam borcunu azalt
        customer.CurrentDebt -= request.Amount;

        var payment = new Entities.Payment
        {
            InvoiceId = request.InvoiceId,
            Amount = request.Amount,
            PaymentDate = DateTime.UtcNow,
            Description = request.Description
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return new PaymentListDto
        {
            Id = payment.Id,
            InvoiceId = payment.InvoiceId,
            CustomerName = $"{customer.Name} {customer.Surname}",
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate,
            Description = payment.Description
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var payment = await _context.Payments
            .Include(p => p.Invoice)
            .ThenInclude(i => i!.Customer)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null)
        {
            return false;
        }

        var invoice = payment.Invoice!;
        var customer = invoice.Customer!;

        // Silinen ödeme tutarını geri ekle
        invoice.PaidAmount -= payment.Amount;
        customer.CurrentDebt += payment.Amount;

        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync();

        return true;
    }
}
