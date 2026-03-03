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

// GHTK Settings
builder.Services.Configure<GhtkSettings>(builder.Configuration.GetSection("GHTK"));

// HttpClient for GHN API
builder.Services.AddHttpClient("GHN");

// HttpClient for GHTK API
builder.Services.AddHttpClient("GHTK");

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
