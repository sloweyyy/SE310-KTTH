namespace WebAdminDashboard.Models
{
  public class AuthResponseModel
  {
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string Role { get; set; } // Add the Role property
  }
}
