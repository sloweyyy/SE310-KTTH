using Microsoft.EntityFrameworkCore;
using WebAdminDashboard.Models;

namespace WebAdminDashboard.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() { }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }
    public DbSet<ProductModel> Products { get; set; }
    public DbSet<LoginModel> Login { get; set; }
    public DbSet<RegisterModel> Register { get; set; }
}
