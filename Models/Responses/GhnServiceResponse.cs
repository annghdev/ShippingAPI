using System.Text.Json.Serialization;

namespace ShippingAPI.Models.Responses;

public class GhnServiceData
{
    [JsonPropertyName("service_id")]
    public int ServiceId { get; set; }

    [JsonPropertyName("short_name")]
    public string ShortName { get; set; } = string.Empty;

    [JsonPropertyName("service_type_id")]
    public int ServiceTypeId { get; set; }
}
