using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using WebAdminDashboard.Models;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WebAdminDashboard.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;

        public AdminController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5159/api/v1/");
        }

        public async Task<IActionResult> Dashboard()
        {
            var products = await _httpClient.GetFromJsonAsync<List<ProductModel>>("Products");

            if (products == null || !products.Any())
            {
                return View(new Tuple<List<ProductModel>, List<string>, List<decimal>, List<double>>(new List<ProductModel>(), new List<string>(), new List<decimal>(), new List<double>()));
            }

            var labels = products.Select(p => p.Name).ToList();
            var prices = products.Select(p => p.Price ?? 0).ToList();
            var ratings = products.Select(p => p.Rating).ToList();

            var chartData = new Tuple<List<ProductModel>, List<string>, List<decimal>, List<double>>(products, labels, prices, ratings);
            return View(chartData);
        }

        public async Task<IActionResult> Products()
        {
            var products = await _httpClient.GetFromJsonAsync<List<ProductModel>>("Products");
            return View(products);
        }
    }
}
