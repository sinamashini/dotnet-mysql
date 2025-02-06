
using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Models;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<IEnumerable<Product>> GetLowStockProducts(int threshold);
}

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.Include(p => p.Supplier).ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.Include(p => p.Supplier).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }
public async Task<Product> UpdateAsync(Product product)
{
    var existingProduct = await _context.Products.Include(p => p.Supplier).FirstOrDefaultAsync(p => p.Id == product.Id);
    if (existingProduct == null) 
        throw new Exception("Product not found");

    if (!string.IsNullOrEmpty(product.Name))
    {
        existingProduct.Name = product.Name;
    }

    if (product.Price > 0)
    {
        existingProduct.Price = product.Price;
    }

    if (product.Quantity > 0 ) 
    {
        existingProduct.Quantity = product.Quantity;
    }

    if (product.SupplierId > 0) 
    {
        var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == product.SupplierId);
        if (!supplierExists)
        {
            throw new Exception("Supplier not found");
        }

        existingProduct.SupplierId = product.SupplierId;

    }
    await _context.SaveChangesAsync();
    return existingProduct;
}

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) throw new Exception("Product not found");
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Product>> GetLowStockProducts(int threshold)
    {
        return await _context.Products.Where(p => p.Quantity < threshold).ToListAsync();
    }
}
