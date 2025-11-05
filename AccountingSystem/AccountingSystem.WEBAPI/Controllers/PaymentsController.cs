using AccountingSystem.WEBAPI.DTOs.Payments;
using AccountingSystem.WEBAPI.Services.Payment.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSystem.WEBAPI.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;

    public PaymentsController(IPaymentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
        {
            return NotFound("Payment not found");
        }
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePaymentDto request)
    {
        var result = await _service.CreateAsync(request);
        return Ok(request);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}
