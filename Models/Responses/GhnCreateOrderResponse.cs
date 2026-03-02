using System.Text.Json.Serialization;

namespace ShippingAPI.Models.Responses;

public class GhnCreateOrderData
{
    [JsonPropertyName("order_code")]
    public string OrderCode { get; set; } = string.Empty;

    [JsonPropertyName("total_fee")]
    public int TotalFee { get; set; }

    [JsonPropertyName("expected_delivery_time")]
    public string? ExpectedDeliveryTime { get; set; }
}
