using System.Text.Json.Serialization;

namespace ShippingAPI.Models.Responses;

public class GhtkCreateOrderData
{
    [JsonPropertyName("partner_id")]
    public string PartnerId { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("area")]
    public string? Area { get; set; }

    [JsonPropertyName("fee")]
    public string? Fee { get; set; }

    [JsonPropertyName("insurance_fee")]
    public string? InsuranceFee { get; set; }

    [JsonPropertyName("tracking_id")]
    public long TrackingId { get; set; }

    [JsonPropertyName("estimated_pick_time")]
    public string? EstimatedPickTime { get; set; }

    [JsonPropertyName("estimated_deliver_time")]
    public string? EstimatedDeliverTime { get; set; }

    [JsonPropertyName("status_id")]
    public int StatusId { get; set; }
}
