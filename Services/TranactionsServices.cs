
using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Models;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task<Transaction?> GetByIdAsync(int id);
    Task<Transaction> ProcessTransactionAsync(Transaction transaction);
    Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalRevenueAsync();
}

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;

    public TransactionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync()
    {
        return await _context.Transactions
            .Include(t => t.Product)
            .ThenInclude(p => p.Supplier)
            .ToListAsync();
    }

    public async Task<Transaction?> GetByIdAsync(int id)
    {
        return await _context.Transactions
            .Include(t => t.Product)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Transaction> ProcessTransactionAsync(Transaction transaction)
    {
        var product = await _context.Products.FindAsync(transaction.ProductId);
        if (product == null) throw new Exception("Product not found");

        if (product.Quantity < transaction.Quantity)
            throw new Exception("Insufficient stock");

        product.Quantity -= transaction.Quantity;
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Transactions
            .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
            .Include(t => t.Product)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        return await _context.Transactions
            .Include(t => t.Product)
            .SumAsync(t => t.Quantity * t.Product.Price);
    }
}
