namespace ShippingAPI.Models.Requests;

public class GhnCreateOrderRequest
{
    public string ToName { get; set; } = string.Empty;
    public string ToPhone { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;
    public string ToWardCode { get; set; } = string.Empty;
    public int ToDistrictId { get; set; }
    public int Weight { get; set; }
    public int Length { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int ServiceId { get; set; }
    public int InsuranceValue { get; set; }
    public int CodAmount { get; set; }
}
