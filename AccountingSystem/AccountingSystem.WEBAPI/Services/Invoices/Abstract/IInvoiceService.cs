using AccountingSystem.WEBAPI.DTOs.Invoices;

namespace AccountingSystem.WEBAPI.Services.Invoices.Abstract;

public interface IInvoiceService
{
    Task<List<InvoiceListDto>> GetAllAsync();
    Task<InvoiceDetailDto?> GetByIdAsync(int id);
    Task<InvoiceDetailDto> CreateAsync(CreateInvoiceDto request);
    Task<bool> DeleteAsync(int id);
}
