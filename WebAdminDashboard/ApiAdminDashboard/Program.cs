using ApiAdminDashboard.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

DotNetEnv.Env.Load();

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
