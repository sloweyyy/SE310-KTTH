using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAdminDashboard.Data;
using WebAdminDashboard.Models;

namespace WebAdminDashboard.Controllers;

public class HomeController : Controller
{
    private readonly HttpClient _httpClient;

    public HomeController(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:5159/api/v1/");
    }

    public async Task<IActionResult> Index()
    {
        var products = await _httpClient.GetFromJsonAsync<List<ProductModel>>("Products");
        return View(products);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
