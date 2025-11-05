using AccountingSystem.WEBAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountingSystem.WEBAPI.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Bütün decimal property'ler için 18,2 ayarı
        foreach (var property in modelBuilder.Model
                     .GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        modelBuilder.Entity<Customer>()
           .HasMany(c => c.Invoices)
           .WithOne(i => i.Customer)
           .HasForeignKey(i => i.CustomerId)
           .OnDelete(DeleteBehavior.Cascade); // Müşteri silinirse faturalar, fatura silinirse kalemler/ödemeler silinir.

        modelBuilder.Entity<Invoice>()
           .HasMany(i => i.InvoiceItems)
           .WithOne(ii => ii.Invoice)
           .HasForeignKey(ii => ii.InvoiceId)
           .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Invoice>()
            .HasMany(i => i.Payments)
            .WithOne(p => p.Invoice)
            .HasForeignKey(p => p.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>()
           .HasMany(p => p.InvoiceItems)
           .WithOne(ii => ii.Product)
           .HasForeignKey(ii => ii.ProductId)
           .OnDelete(DeleteBehavior.Restrict); //Ürün silinirse fatura kalemi silinmesin (veri kaybı önlenir).
    }
}
