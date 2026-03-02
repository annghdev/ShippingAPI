using System.ComponentModel.DataAnnotations;

namespace ShippingAPI.Entities;

public class OrderTracking
{
    [Key]
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
