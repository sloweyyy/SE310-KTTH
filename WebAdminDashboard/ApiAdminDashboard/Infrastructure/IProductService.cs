using ApiAdminDashboard.Models;
using ApiAdminDashboard.Data.Models;

namespace ApiAdminDashboard.Infrastructure
{
  public interface IProductService
  {
    Task<List<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> AddProductAsync(AddProductDto addProductDto);
    Task<Product> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
    Task DeleteProductAsync(int id);
  }
}