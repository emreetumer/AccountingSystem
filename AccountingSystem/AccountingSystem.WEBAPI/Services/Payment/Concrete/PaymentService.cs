using AccountingSystem.WEBAPI.Context;
using AccountingSystem.WEBAPI.DTOs.Payments;
using AccountingSystem.WEBAPI.Hubs;
using AccountingSystem.WEBAPI.Services.Payment.Abstract;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AccountingSystem.WEBAPI.Services.Payment.Concrete;

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificationHub> _hub;

    public PaymentService(ApplicationDbContext context, IHubContext<NotificationHub> hub)
    {
        _context = context;
        _hub = hub;
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
            KalanAmount = p.Invoice.Customer.CurrentDebt,
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
        invoice.PaidAmount = invoice.PaidAmount + request.Amount;

        if (invoice.PaidAmount > invoice.TotalAmount)
        {
            throw new InvalidOperationException($"Bu faturanın toplam borcu {invoice.Customer!.CurrentDebt}dur. Siz {invoice.PaidAmount - invoice.TotalAmount} lira kadar fazla ödeme yaptınız. Lütfen borcunuz kadar ödeme yapınız ");
        }

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

        #region SignalR
        // 1) Ödeme alındı olayı (dashboard + ilgili müşteri grubu)
        await _hub.Clients.Group("dashboard").SendAsync("PaymentReceived", new
        {
            paymentId = payment.Id,
            invoiceId = invoice.Id,
            customerId = customer.Id,
            amount = payment.Amount,
            paymentDate = payment.PaymentDate
        });

        await _hub.Clients.Group($"customer-{customer.Id}").SendAsync("PaymentReceived", new
        {
            paymentId = payment.Id,
            invoiceId = invoice.Id,
            amount = payment.Amount,
            paymentDate = payment.PaymentDate
        });

        // 2) Müşteri borcu güncellendi olayı
        await _hub.Clients.Group($"customer-{customer.Id}").SendAsync("CustomerDebtUpdated", new
        {
            customerId = customer.Id,
            currentDebt = customer.CurrentDebt
        });

        #endregion

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
