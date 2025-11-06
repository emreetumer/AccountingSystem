using Microsoft.AspNetCore.SignalR;

namespace AccountingSystem.WEBAPI.Hubs;

public class NotificationHub : Hub
{
    // İsteğe bağlı: client'ı belirli gruplara almak için çağrılır (ör: müşteri bazlı)
    public async Task JoinCustomerGroup(int customerId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"customer-{customerId}");
    }

    // Yönetim ekranı / dashboard için genel grup
    public async Task JoinDashboard()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "dashboard");
    }
}
