using System.Text.Json.Serialization;

namespace ShippingAPI.Models.Responses;

public class GhnFeeData
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("service_fee")]
    public int ServiceFee { get; set; }

    [JsonPropertyName("insurance_fee")]
    public int InsuranceFee { get; set; }

    [JsonPropertyName("pick_station_fee")]
    public int PickStationFee { get; set; }

    [JsonPropertyName("coupon_value")]
    public int CouponValue { get; set; }

    [JsonPropertyName("r2s_fee")]
    public int R2sFee { get; set; }
}
