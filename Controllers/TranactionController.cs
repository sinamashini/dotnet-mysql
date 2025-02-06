using Microsoft.AspNetCore.Mvc;
using MyWebApi.Models;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var transactions = await _transactionService.GetAllAsync();
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var transaction = await _transactionService.GetByIdAsync(id);
        if (transaction == null) return NotFound();
        return Ok(transaction);
    }

    [HttpPost]
    public async Task<IActionResult> ProcessTransaction(Transaction transaction)
    {
        try
        {
            var processedTransaction = await _transactionService.ProcessTransactionAsync(transaction);
            return CreatedAtAction(nameof(GetById), new { id = processedTransaction.Id }, processedTransaction);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("date-range")]
    public async Task<IActionResult> GetTransactionsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var transactions = await _transactionService.GetTransactionsByDateRangeAsync(startDate, endDate);
        return Ok(transactions);
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetTotalRevenue()
    {
        var totalRevenue = await _transactionService.GetTotalRevenueAsync();
        return Ok(new { TotalRevenue = totalRevenue });
    }
}
