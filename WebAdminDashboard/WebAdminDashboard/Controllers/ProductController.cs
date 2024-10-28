using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebAdminDashboard.Models;

namespace WebAdminDashboard.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5159/api/v1/");
        }

        public async Task<IActionResult> Index()
        {
            var products = await _httpClient.GetFromJsonAsync<List<ProductModel>>("Products");
            return View(products);
        }

        public async Task<IActionResult> ProductDetail(int id)
        {
            var product = await _httpClient.GetFromJsonAsync<ProductModel>($"Products/{id}");
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync("Products", product);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Error creating product. Please try again.");
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _httpClient.GetFromJsonAsync<ProductModel>($"Products/{id}");
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductModel product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var response = await _httpClient.PutAsJsonAsync($"Products/{id}", product);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Error updating product. Please try again.");
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"Products/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error deleting product. Please try again.");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Products()
        {
            var products = await _httpClient.GetFromJsonAsync<List<ProductModel>>("Products");
            return View(products);
        }
    }
}