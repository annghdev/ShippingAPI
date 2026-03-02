namespace ShippingAPI.Models.Settings;

public class GhnSettings
{
    public string Token { get; set; } = string.Empty;
    public int ShopId { get; set; }
    public string GetProvincesUrl { get; set; } = string.Empty;
    public string GetDistrictsUrl { get; set; } = string.Empty;
    public string GetWardsUrl { get; set; } = string.Empty;
    public string GetAvailableServicesUrl { get; set; } = string.Empty;
    public string CalculateShippingFeeUrl { get; set; } = string.Empty;
    public string CreateOrderUrl { get; set; } = string.Empty;
    public string TrackingOrderUrl { get; set; } = string.Empty;
}
