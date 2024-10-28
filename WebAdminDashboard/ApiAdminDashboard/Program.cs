using ApiAdminDashboard.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables (if needed)
DotNetEnv.Env.Load();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("Data Source=localhost; Initial Catalog=KTTH; User ID=sa; Password=SloWeydadev20062004@; TrustServerCertificate=True"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();