using Microsoft.AspNetCore.Mvc;
using WebAdminDashboard.Models;

namespace WebAdminDashboard.Controllers
{
  public class AccountController : Controller
  {
    public IActionResult Login()
    {
      return View();
    }

    [HttpPost]
    public IActionResult Login(LoginModel model)
    {
      if (ModelState.IsValid)
      {
        // Logic to authenticate the user
        // ...

        // Get the user's role (replace with your actual logic)
        string userRole = GetUserRole(model.Email);

        // Redirect based on the role
        if (userRole == "user")
        {
          return RedirectToAction("Index", "Home");
        }
        else if (userRole == "admin")
        {
          return RedirectToAction("Index", "Dashboard");
        }
        else
        {
          // Handle invalid role
          return View("Error");
        }
      }

      // If authentication fails, display the login form again with error messages
      return View(model);
    }

    // Placeholder method to get the user's role
    private string GetUserRole(string email)
    {
      // Replace this with your actual logic to retrieve the user's role from your database or authentication system
      // For example, you could use a database query or an API call to get the role.
      // ...

      // Return the user's role
      return "user"; // Replace with the actual role
    }

    public IActionResult Register()
    {
      return View();
    }

    [HttpPost]
    public IActionResult Register(RegisterModel model)
    {
      if (ModelState.IsValid)
      {
        // Logic to register the user
        // ...

        // If registration is successful, redirect to the login page
        return RedirectToAction("Login");
      }

      // If registration fails, display the registration form again with error messages
      return View(model);
    }
  }
}