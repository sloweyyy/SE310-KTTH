using Microsoft.AspNetCore.Mvc;
using WebAdminDashboard.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebAdminDashboard.Data;
using Microsoft.Extensions.Logging;


namespace WebAdminDashboard.Controllers
{
  public class AccountController : Controller
  {
    private readonly HttpClient _httpClient;
    private readonly ILogger<AccountController> _logger; // Add the logger

    public AccountController(HttpClient httpClient, ILogger<AccountController> logger) // Add the context to the constructor
    {
      _httpClient = httpClient;
      _httpClient.BaseAddress = new Uri("http://localhost:5159/api/v1/");
      _logger = logger;
    }

    public IActionResult Login()
    {
      return View();
    }

    public IActionResult Register()
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
          var responseContent = await response.Content.ReadAsStringAsync();
          Console.WriteLine("Raw API response: " + responseContent); // Log raw response

          var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

          // Then check if authResponse has the role populated
          // Log the role for debugging
          Console.WriteLine("Logged in role after deserialization: " + authResponse?.Role);

          // Ensure the token is not null
          if (authResponse?.Token != null)
          {
            HttpContext.Session.SetString("JwtToken", authResponse.Token);
            HttpContext.Session.SetString("RefreshToken", authResponse.RefreshToken);

            // Set ViewBag for frontend access
            ViewBag.Role = authResponse.Role;

            // Redirect based on role
            if (authResponse.Role == "user")
            {
              return Redirect("http://localhost:5056/"); // Redirect to user area
            }
            else if (authResponse.Role == "admin")
            {
              return Redirect("http://localhost:5056/Admin/Dashboard"); // Redirect to admin area
            }
          }


        }
        else
        {
          // Read the content for error details
          var errorContent = await response.Content.ReadAsStringAsync();
          ModelState.AddModelError("", errorContent); // Log the error content for debugging
        }
      }
      return View(loginModel);
    }





    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterModel registerModel)
    {
      if (ModelState.IsValid)
      {
        // Call the backend register API instead of adding to the database directly
        var response = await _httpClient.PostAsJsonAsync("Auth/register", registerModel);

        if (response.IsSuccessStatusCode)
        {
          return RedirectToAction("Login");
        }
        else
        {
          // Read the content for error details
          var errorContent = await response.Content.ReadAsStringAsync();
          ModelState.AddModelError("", errorContent); // Log the error content for debugging
        }
      }

      ModelState.AddModelError("", "Invalid registration attempt.");
      return View(registerModel);
    }

    public async Task<IActionResult> Logout()
    {
      // Optionally, call the backend to revoke the token
      var refreshToken = HttpContext.Session.GetString("RefreshToken");
      if (!string.IsNullOrEmpty(refreshToken))
      {
        var tokenResponse = await _httpClient.PostAsJsonAsync("Auth/revoke-token", new { refreshToken });
      }

      HttpContext.Session.Remove("JwtToken");
      HttpContext.Session.Remove("RefreshToken");

      return RedirectToAction("Login", "Auth");
    }
  }

  public class AuthResponse
  {
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string Role { get; set; }
  }
}
