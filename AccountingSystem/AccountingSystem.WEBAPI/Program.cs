using AccountingSystem.WEBAPI.Context;
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
