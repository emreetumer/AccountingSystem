using AccountingSystem.WEBAPI.DTOs.Payments;

namespace AccountingSystem.WEBAPI.Services.Payment.Abstract;

public interface IPaymentService
{
    Task<List<PaymentListDto>> GetAllAsync();
    Task<PaymentListDto?> GetByIdAsync(int id);
    Task<PaymentListDto> CreateAsync(CreatePaymentDto request);
    Task<bool> DeleteAsync(int id);
}
