using System.Text.Json.Serialization;

namespace ShippingAPI.Models.Requests;

public class GhtkCreateOrderRequest
{
    // Thông tin người nhận
    public string Name { get; set; } = string.Empty;        // Tên người nhận
    public string Tel { get; set; } = string.Empty;          // SĐT người nhận
    public string Address { get; set; } = string.Empty;      // Địa chỉ người nhận
    public string Province { get; set; } = string.Empty;     // Tỉnh/TP người nhận
    public string District { get; set; } = string.Empty;     // Quận/Huyện người nhận
    public string Ward { get; set; } = string.Empty;         // Phường/Xã người nhận

    // Thông tin đơn hàng
    public int PickMoney { get; set; }                       // Tiền thu hộ (COD)
    public int Value { get; set; }                           // Giá trị hàng hóa (VNĐ)
    public int Weight { get; set; }                          // Khối lượng (gram)
    public string Transport { get; set; } = "road";          // "road" hoặc "fly"

    // Sản phẩm
    public string ProductName { get; set; } = "Đơn hàng";
    public int ProductQuantity { get; set; } = 1;
}
