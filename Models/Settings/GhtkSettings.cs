namespace ShippingAPI.Models.Settings;

public class GhtkSettings
{
    public string Token { get; set; } = string.Empty;
    public string CreateOrderUrl { get; set; } = string.Empty;
    public string CalculateFeeUrl { get; set; } = string.Empty;
    public string TrackingBaseUrl { get; set; } = string.Empty;
}
