using System.ComponentModel.DataAnnotations;

namespace ShippingAPI.Entities;

public class Order
{
    [Key]
    public Guid Id { get; set; }

    // Recipient info
    public string ToName { get; set; } = string.Empty;
    public string ToPhone { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;
    public string ToWardCode { get; set; } = string.Empty;
    public int ToDistrictId { get; set; }

    // Package dimensions
    public int Weight { get; set; }   // gram
    public int Length { get; set; }    // cm
    public int Width { get; set; }     // cm
    public int Height { get; set; }    // cm

    // Shipping details
    public int ServiceId { get; set; }
    public int InsuranceValue { get; set; }
    public int CodAmount { get; set; }

    // Shipping provider
    public ShippingProvider Provider { get; set; } = ShippingProvider.GHN;

    // GHN response data
    public string? GhnOrderCode { get; set; }

    // GHTK response data
    public string? GhtkLabel { get; set; }
    public long? GhtkTrackingId { get; set; }

    public decimal? ShippingFee { get; set; }

    // Status
    public ShippingStatus Status { get; set; } = ShippingStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
