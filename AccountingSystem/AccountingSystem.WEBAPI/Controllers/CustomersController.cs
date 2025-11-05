using AccountingSystem.WEBAPI.DTOs.Customers;
using AccountingSystem.WEBAPI.Services.Customer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSystem.WEBAPI.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _service;
    public CustomersController(ICustomerService service)
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
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCustomerDto request)
    {
        var result = await _service.CreateAsync(request);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCustomerDto request)
    {
        var result = await _service.UpdateAsync(request);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }

}
