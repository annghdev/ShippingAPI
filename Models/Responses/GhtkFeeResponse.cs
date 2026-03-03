using System.Text.Json.Serialization;

namespace ShippingAPI.Models.Responses;

public class GhtkFeeData
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("fee")]
    public int Fee { get; set; }

    [JsonPropertyName("insurance_fee")]
    public int InsuranceFee { get; set; }

    [JsonPropertyName("delivery_type")]
    public string? DeliveryType { get; set; }

    [JsonPropertyName("delivery")]
    public bool Delivery { get; set; }

    [JsonPropertyName("extFees")]
    public List<GhtkExtFee>? ExtFees { get; set; }
}

public class GhtkExtFee
{
    [JsonPropertyName("display")]
    public string? Display { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("amount")]
    public int Amount { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}
