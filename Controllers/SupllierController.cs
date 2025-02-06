using Microsoft.AspNetCore.Mvc;
using MyWebApi.Models;

[ApiController]
[Route("api/[controller]")]
public class SupplierController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SupplierController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var suppliers = await _supplierService.GetAllAsync();
        return Ok(suppliers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var supplier = await _supplierService.GetByIdAsync(id);
        if (supplier == null) return NotFound();
        return Ok(supplier);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Supplier supplier)
    {
        var createdSupplier = await _supplierService.CreateAsync(supplier);
        return CreatedAtAction(nameof(GetById), new { id = createdSupplier.Id }, createdSupplier);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Supplier supplier)
    {
        if (id != supplier.Id) return BadRequest();

        try
        {
            await _supplierService.UpdateAsync(supplier);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _supplierService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}
