
using AccountingSystem.WEBAPI.Context;
using AccountingSystem.WEBAPI.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AccountingSystem.WEBAPI.BackgroundWorker;

public class OverdueInvoiceWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<NotificationHub> _hub;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

    public OverdueInvoiceWorker(IServiceProvider serviceProvider, IHubContext<NotificationHub> hub)
    {
        _serviceProvider = serviceProvider;
        _hub = hub;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Vadesi geçmiş ve ödenmemiş faturalar
            var now = DateTime.UtcNow.Date;
            var overdue = await db.Invoices
                .Include(i => i.Customer)
                .Where(i => i.DueDate.Date < now && i.TotalAmount > i.PaidAmount)
                .ToListAsync(stoppingToken);

            foreach (var inv in overdue)
            {
                // Dashboard’a genel uyarı
                await _hub.Clients.Group("dashboard").SendAsync("OverdueInvoiceDetected", new
                {
                    invoiceId = inv.Id,
                    customerId = inv.CustomerId,
                    customerName = $"{inv.Customer!.Name} {inv.Customer!.Surname}",
                    totalAmount = inv.TotalAmount,
                    paidAmount = inv.PaidAmount,
                    dueDate = inv.DueDate
                }, cancellationToken: stoppingToken);

                // İlgili müşteri grubuna uyarı
                await _hub.Clients.Group($"customer-{inv.CustomerId}").SendAsync("OverdueInvoiceDetected", new
                {
                    invoiceId = inv.Id,
                    totalAmount = inv.TotalAmount,
                    paidAmount = inv.PaidAmount,
                    dueDate = inv.DueDate
                }, cancellationToken: stoppingToken);
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}
