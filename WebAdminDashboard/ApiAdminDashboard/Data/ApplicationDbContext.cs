using Microsoft.EntityFrameworkCore;
using ApiAdminDashboard.Data.Models;

namespace ApiAdminDashboard.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() { }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<Product> Products { get; set; }
}

