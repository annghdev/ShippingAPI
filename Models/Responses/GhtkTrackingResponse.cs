using System.Text.Json.Serialization;

namespace ShippingAPI.Models.Responses;

public class GhtkTrackingData
{
    [JsonPropertyName("label_id")]
    public string LabelId { get; set; } = string.Empty;

    [JsonPropertyName("partner_id")]
    public string PartnerId { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("status_text")]
    public string StatusText { get; set; } = string.Empty;

    [JsonPropertyName("created")]
    public string? Created { get; set; }

    [JsonPropertyName("modified")]
    public string? Modified { get; set; }

    [JsonPropertyName("pick_date")]
    public string? PickDate { get; set; }

    [JsonPropertyName("deliver_date")]
    public string? DeliverDate { get; set; }

    [JsonPropertyName("customer_fullname")]
    public string? CustomerFullname { get; set; }

    [JsonPropertyName("customer_tel")]
    public string? CustomerTel { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("ship_money")]
    public string? ShipMoney { get; set; }

    [JsonPropertyName("pick_money")]
    public int PickMoney { get; set; }

    [JsonPropertyName("is_freeship")]
    public string? IsFreeship { get; set; }
}
