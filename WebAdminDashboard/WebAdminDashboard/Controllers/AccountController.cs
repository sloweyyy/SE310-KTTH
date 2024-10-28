using Microsoft.AspNetCore.Mvc;
using WebAdminDashboard.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebAdminDashboard.Controllers
{
  public class AccountController : Controller
  {
    private readonly HttpClient _httpClient;

    public AccountController(HttpClient httpClient)
    {
      _httpClient = httpClient;
      _httpClient.BaseAddress = new Uri("http://localhost:5159/api/");
    }

    public IActionResult Login()
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel loginModel)
    {
      if (ModelState.IsValid)
      {
        var response = await _httpClient.PostAsJsonAsync("Auth/login", loginModel);

        if (response.IsSuccessStatusCode)
        {
          // Use AuthResponseModel instead of AuthResponse
          var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseModel>();
          HttpContext.Session.SetString("JwtToken", authResponse.Token); // Corrected property name

          // Redirect based on role
          if (authResponse.Role == "user")
          {
            return RedirectToAction("Index", "Product");
          }
          else if (authResponse.Role == "admin")
          {
            return RedirectToAction("Dashboard", "Admin");
          }
        }

        ModelState.AddModelError("", "Invalid login attempt.");
      }
      return View(loginModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterModel registerModel)
    {
      if (ModelState.IsValid)
      {
        var response = await _httpClient.PostAsJsonAsync("Auth/register", registerModel);

        if (response.IsSuccessStatusCode)
        {
          return RedirectToAction("Login");
        }

        ModelState.AddModelError("", "Invalid registration attempt.");
      }
      return View(registerModel);
    }


    public IActionResult Logout()
    {
      HttpContext.Session.Remove("JwtToken");
      return RedirectToAction("Login", "Auth");
    }
  }


  public class AuthResponse
  {
    public string Token { get; set; }
    public string RefreshToken { get; set; }
  }
}