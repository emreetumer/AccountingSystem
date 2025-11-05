using AccountingSystem.WEBAPI.DTOs.Invoices;
using AccountingSystem.WEBAPI.Services.Invoices.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSystem.WEBAPI.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _service;

    public InvoicesController(IInvoiceService service)
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
            return NotFound("Invoice not found");
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateInvoiceDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}
