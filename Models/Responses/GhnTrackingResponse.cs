using System.Text.Json.Serialization;

namespace ShippingAPI.Models.Responses;

public class GhnTrackingData
{
    [JsonPropertyName("order_code")]
    public string OrderCode { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("log")]
    public List<GhnTrackingLog> Log { get; set; } = [];
}

public class GhnTrackingLog
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("updated_date")]
    public string? UpdatedDate { get; set; }
}
