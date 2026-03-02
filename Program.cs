using Microsoft.EntityFrameworkCore;
using ShippingAPI.Data;
using ShippingAPI.Models.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// EF Core InMemory
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ShippingDb"));

// GHN Settings
builder.Services.Configure<GhnSettings>(builder.Configuration.GetSection("GHN"));

// HttpClient for GHN API
builder.Services.AddHttpClient("GHN");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();

// Make Program class accessible for WebApplicationFactory in tests
public partial class Program { }
