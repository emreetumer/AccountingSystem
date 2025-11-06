using AccountingSystem.WEBAPI.BackgroundWorker;
using AccountingSystem.WEBAPI.Context;
using AccountingSystem.WEBAPI.Hubs;
using AccountingSystem.WEBAPI.Services.Customer.Abstract;
using AccountingSystem.WEBAPI.Services.Customer.Concrete;
using AccountingSystem.WEBAPI.Services.Invoices.Abstract;
using AccountingSystem.WEBAPI.Services.Invoices.Concrete;
using AccountingSystem.WEBAPI.Services.Payment.Abstract;
using AccountingSystem.WEBAPI.Services.Payment.Concrete;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();


builder.Services.AddSignalR();

builder.Services.AddHostedService<OverdueInvoiceWorker>(); // Vade Kontrolü


builder.Services.AddCors(o =>
{
    o.AddPolicy("frontend", p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed(_ => true));
});

var app = builder.Build();

app.UseCors("frontend");


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();
