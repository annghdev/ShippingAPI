namespace ShippingAPI.Models.Requests;

public class GhtkCalculateFeeRequest
{
    public string PickProvince { get; set; } = string.Empty;    // Tỉnh/TP lấy hàng
    public string PickDistrict { get; set; } = string.Empty;    // Quận/Huyện lấy hàng
    public string Province { get; set; } = string.Empty;        // Tỉnh/TP giao hàng
    public string District { get; set; } = string.Empty;        // Quận/Huyện giao hàng
    public string Address { get; set; } = string.Empty;         // Địa chỉ giao hàng
    public int Weight { get; set; }                             // Khối lượng (gram)
    public int Value { get; set; }                              // Giá trị hàng (VNĐ)
    public string Transport { get; set; } = "road";             // "road" hoặc "fly"
}
