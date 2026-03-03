using System.Text.Json.Serialization;

namespace ShippingAPI.Models.Responses;

public class GhtkResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("order")]
    public T? Order { get; set; }
}

public class GhtkFeeResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("fee")]
    public GhtkFeeData? Fee { get; set; }
}
