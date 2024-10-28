using System.Linq;
using ApiAdminDashboard.Data;
using ApiAdminDashboard.Models;
using ApiAdminDashboard.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiAdminDashboard.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ProductsController(ApplicationDbContext dbContext)
        {
            _db = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllProducts()
        {
            return Ok(_db.Products.ToList());
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetProduct(int id)
        {
            var product = _db.Products.Find(id);
            if (product is null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public IActionResult AddProduct(AddProductDto addProductDto)
        {
            var productEntity = new Product()
            {
                Name = addProductDto.Name,
                Description = addProductDto.Description,
                ImageUrl = addProductDto.ImageUrl,
                Price = addProductDto.Price
            };

            _db.Products.Add(productEntity);
            _db.SaveChanges();

            return Ok(productEntity);
        }

        [HttpPut]
        [Route("{id:int}")]
        public IActionResult UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            var productEntity = _db.Products.Find(id);
            if (productEntity is null)
            {
                return NotFound();
            }
            productEntity.Name = updateProductDto.Name;
            productEntity.Description = updateProductDto.Description;
            productEntity.Price = updateProductDto.Price;
            productEntity.ImageUrl = updateProductDto.ImageUrl;

            _db.SaveChanges();

            return Ok(productEntity);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult DeleteProduct(int id)
        {
            var productEntity = _db.Products.Find(id);

            if (productEntity is null)
            {
                return NotFound();
            }

            _db.Products.Remove(productEntity);

            _db.SaveChanges();

            return Ok(GetAllProducts());
        }
    }
}