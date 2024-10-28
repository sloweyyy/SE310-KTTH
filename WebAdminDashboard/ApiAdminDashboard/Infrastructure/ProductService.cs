using ApiAdminDashboard.Models;
using ApiAdminDashboard.Data.Models;
using ApiAdminDashboard.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiAdminDashboard.Infrastructure
{
  public class ProductService : IProductService
  {
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
      return await _context.Products.ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
      return await _context.Products.FindAsync(id);
    }

    public async Task<Product> AddProductAsync(AddProductDto addProductDto)
    {
      var product = new Product
      {
        Name = addProductDto.Name,
        Description = addProductDto.Description,
        ImageUrl = addProductDto.ImageUrl,
        Price = addProductDto.Price
      };

      _context.Products.Add(product);
      await _context.SaveChangesAsync();

      return product;
    }

    public async Task<Product> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
    {
      var product = await _context.Products.FindAsync(id);

      if (product == null)
      {
        throw new ArgumentException($"Product with ID {id} not found.");
      }

      product.Name = updateProductDto.Name;
      product.Description = updateProductDto.Description;
      product.ImageUrl = updateProductDto.ImageUrl;
      product.Price = updateProductDto.Price;

      await _context.SaveChangesAsync();

      return product;
    }

    public async Task DeleteProductAsync(int id)
    {
      var product = await _context.Products.FindAsync(id);

      if (product == null)
      {
        throw new ArgumentException($"Product with ID {id} not found.");
      }

      _context.Products.Remove(product);
      await _context.SaveChangesAsync();
    }
  }
}