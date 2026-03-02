using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ShippingAPI.Data;
using ShippingAPI.Entities;
using ShippingAPI.Models.Requests;
using ShippingAPI.Models.Responses;
using ShippingAPI.Models.Settings;

namespace ShippingAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShippingsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GhnSettings _ghn;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public ShippingsController(
        AppDbContext db,
        IHttpClientFactory httpClientFactory,
        IOptions<GhnSettings> ghnOptions)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
        _ghn = ghnOptions.Value;
    }

    // ──────────────────────────────────────────────
    // 1. POST /api/shippings/orders  – Tạo đơn giao hàng
    // ──────────────────────────────────────────────
    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var client = _httpClientFactory.CreateClient("GHN");

        var ghnBody = new
        {
            shop_id = _ghn.ShopId,
            from_name = "Test Shop",
            from_phone = "0987654321",
            from_address = "72 Lê Thánh Tôn, Phường Bến Nghé, Quận 1, Hồ Chí Minh",
            from_ward_code = "21012",
            from_district_id = 1442,
            return_phone = "0987654321",
            return_address = "72 Lê Thánh Tôn, Phường Bến Nghé, Quận 1, Hồ Chí Minh",
            return_district_id = 1442,
            return_ward_code = "21012",
            to_name = request.ToName,
            to_phone = request.ToPhone,
            to_address = request.ToAddress,
            to_ward_code = request.ToWardCode,
            to_district_id = request.ToDistrictId,
            weight = request.Weight,
            length = request.Length,
            width = request.Width,
            height = request.Height,
            service_type_id = 2, // 2 = Standard shipping
            payment_type_id = 1, // Người gửi trả phí
            required_note = "CHOXEMHANGKHONGTHU",
            cod_amount = request.CodAmount,
            insurance_value = request.InsuranceValue,
            items = new[]
            {
                new { name = "Đơn hàng", quantity = 1, weight = request.Weight }
            }
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, _ghn.CreateOrderUrl)
        {
            Content = JsonContent.Create(ghnBody)
        };
        httpRequest.Headers.Add("Token", _ghn.Token);
        httpRequest.Headers.Add("ShopId", _ghn.ShopId.ToString());

        var response = await client.SendAsync(httpRequest);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return BadRequest(new { error = "GHN API failed", detail = json });

        var ghnResult = JsonSerializer.Deserialize<GhnResponse<GhnCreateOrderData>>(json, _jsonOptions);
        if (ghnResult?.Code != 200 || ghnResult.Data is null)
            return BadRequest(new { error = ghnResult?.Message, detail = json });

        // Lưu order vào DB
        var order = new Order
        {
            Id = Guid.NewGuid(),
            ToName = request.ToName,
            ToPhone = request.ToPhone,
            ToAddress = request.ToAddress,
            ToWardCode = request.ToWardCode,
            ToDistrictId = request.ToDistrictId,
            Weight = request.Weight,
            Length = request.Length,
            Width = request.Width,
            Height = request.Height,
            ServiceId = request.ServiceId,
            InsuranceValue = request.InsuranceValue,
            CodAmount = request.CodAmount,
            GhnOrderCode = ghnResult.Data.OrderCode,
            ShippingFee = ghnResult.Data.TotalFee,
            Status = ShippingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            order.Id,
            order.GhnOrderCode,
            order.ShippingFee,
            ghnResult.Data.ExpectedDeliveryTime,
            order.Status
        });
    }

    // ──────────────────────────────────────────────
    // 2. POST /api/shippings/calculate-fee  – Tính phí ship
    // ──────────────────────────────────────────────
    [HttpPost("calculate-fee")]
    public async Task<IActionResult> CalculateFee([FromBody] CalculateFeeRequest request)
    {
        var client = _httpClientFactory.CreateClient("GHN");

        var ghnBody = new
        {
            shop_id = _ghn.ShopId,
            from_district_id = request.FromDistrictId,
            from_ward_code = request.FromWardCode,
            to_district_id = request.ToDistrictId,
            to_ward_code = request.ToWardCode,
            weight = request.Weight,
            length = request.Length,
            width = request.Width,
            height = request.Height,
            service_id = request.ServiceId,
            insurance_value = request.InsuranceValue
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, _ghn.CalculateShippingFeeUrl)
        {
            Content = JsonContent.Create(ghnBody)
        };
        httpRequest.Headers.Add("Token", _ghn.Token);
        httpRequest.Headers.Add("ShopId", _ghn.ShopId.ToString());

        var response = await client.SendAsync(httpRequest);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return BadRequest(new { error = "GHN API failed", detail = json });

        var ghnResult = JsonSerializer.Deserialize<GhnResponse<GhnFeeData>>(json, _jsonOptions);
        if (ghnResult?.Code != 200 || ghnResult.Data is null)
            return BadRequest(new { error = ghnResult?.Message, detail = json });

        return Ok(ghnResult.Data);
    }

    // ──────────────────────────────────────────────
    // 3. GET /api/shippings/orders/{id}/tracking  – Theo dõi trạng thái
    // ──────────────────────────────────────────────
    [HttpGet("orders/{id:guid}/tracking")]
    public async Task<IActionResult> GetTracking(Guid id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order is null)
            return NotFound(new { error = "Order not found" });

        if (string.IsNullOrEmpty(order.GhnOrderCode))
            return BadRequest(new { error = "Order has no GHN tracking code" });

        var client = _httpClientFactory.CreateClient("GHN");

        var ghnBody = new { order_code = order.GhnOrderCode };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, _ghn.TrackingOrderUrl)
        {
            Content = JsonContent.Create(ghnBody)
        };
        httpRequest.Headers.Add("Token", _ghn.Token);

        var response = await client.SendAsync(httpRequest);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return BadRequest(new { error = "GHN API failed", detail = json });

        var ghnResult = JsonSerializer.Deserialize<GhnResponse<GhnTrackingData>>(json, _jsonOptions);
        if (ghnResult?.Code != 200 || ghnResult.Data is null)
            return BadRequest(new { error = ghnResult?.Message, detail = json });

        return Ok(new
        {
            order.Id,
            order.GhnOrderCode,
            ghnResult.Data.Status,
            ghnResult.Data.Log
        });
    }

    // ──────────────────────────────────────────────
    // 4. GET /api/shippings/services  – Lấy dịch vụ vận chuyển
    // ──────────────────────────────────────────────
    [HttpGet("services")]
    public async Task<IActionResult> GetServices(
        [FromQuery] int from_district,
        [FromQuery] int to_district)
    {
        var client = _httpClientFactory.CreateClient("GHN");

        var ghnBody = new
        {
            shop_id = _ghn.ShopId,
            from_district = from_district,
            to_district = to_district
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, _ghn.GetAvailableServicesUrl)
        {
            Content = JsonContent.Create(ghnBody)
        };
        httpRequest.Headers.Add("Token", _ghn.Token);

        var response = await client.SendAsync(httpRequest);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return BadRequest(new { error = "GHN API failed", detail = json });

        var ghnResult = JsonSerializer.Deserialize<GhnResponse<List<GhnServiceData>>>(json, _jsonOptions);
        if (ghnResult?.Code != 200 || ghnResult.Data is null)
            return BadRequest(new { error = ghnResult?.Message, detail = json });

        return Ok(ghnResult.Data);
    }
}
