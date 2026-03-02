using Microsoft.EntityFrameworkCore;
using ShippingAPI.Entities;

namespace ShippingAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderTracking> OrderTrackings => Set<OrderTracking>();
}
