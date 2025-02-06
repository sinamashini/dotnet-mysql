
using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Models;
public interface ISupplierService
{
    Task<IEnumerable<Supplier>> GetAllAsync();
    Task<Supplier?> GetByIdAsync(int id);
    Task<Supplier> CreateAsync(Supplier supplier);
    Task UpdateAsync(Supplier supplier);
    Task DeleteAsync(int id);
}

public class SupplierService : ISupplierService
{
    private readonly ApplicationDbContext _context;

    public SupplierService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync()
    {
        return await _context.Suppliers
            .Include(s => s.Products)
            .ToListAsync();
    }

    public async Task<Supplier?> GetByIdAsync(int id)
    {
        return await _context.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Supplier> CreateAsync(Supplier supplier)
    {
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();
        return supplier;
    }

    public async Task UpdateAsync(Supplier supplier)
    {
        _context.Suppliers.Update(supplier);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (supplier == null) throw new Exception("Supplier not found");

        if (supplier.Products != null && supplier.Products.Any())
            throw new Exception("Cannot delete a supplier with associated products");

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();
    }
}
