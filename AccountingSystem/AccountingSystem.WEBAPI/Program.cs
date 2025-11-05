using AccountingSystem.WEBAPI.Context;
using AccountingSystem.WEBAPI.Services.Customer.Abstract;
using AccountingSystem.WEBAPI.Services.Customer.Concrete;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddScoped<ICustomerService, CustomerService>();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
