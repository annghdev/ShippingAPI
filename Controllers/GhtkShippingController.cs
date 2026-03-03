using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ShippingAPI.Data;
using ShippingAPI.Entities;
using ShippingAPI.Models.Requests;
using ShippingAPI.Models.Responses;
using ShippingAPI.Models.Settings;

namespace ShippingAPI.Controllers;

[Route("api/shipping/ghtk")]
[ApiController]
public class GhtkShippingController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GhtkSettings _ghtk;

    public GhtkShippingController(
        AppDbContext db,
        IHttpClientFactory httpClientFactory,
        IOptions<GhtkSettings> ghtkOptions)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
        _ghtk = ghtkOptions.Value;
    }

    // ──────────────────────────────────────────────
    // 1. POST /api/shipping/ghtk/orders  – Tạo đơn giao hàng GHTK
    // ──────────────────────────────────────────────
    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder([FromBody] GhtkCreateOrderRequest request)
    {
        var client = _httpClientFactory.CreateClient("GHTK");

        var ghtkBody = new
        {
            products = new[]
            {
                new
                {
                    name = request.ProductName,
                    weight = request.Weight / 1000.0, // GHTK dùng kg
                    quantity = request.ProductQuantity
                }
            },
            order = new
            {
                id = Guid.NewGuid().ToString("N")[..12],
                pick_name = "Test Shop",
                pick_address = "590 CMT8 P.11",
                pick_province = "TP. Hồ Chí Minh",
                pick_district = "Quận 3",
                pick_ward = "Phường 1",
                pick_tel = "0987654321",
                name = request.Name,
                tel = request.Tel,
                address = request.Address,
                province = request.Province,
                district = request.District,
                ward = request.Ward,
                is_freeship = "0",
                pick_money = request.PickMoney,
                value = request.Value,
                transport = request.Transport
            }
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, _ghtk.CreateOrderUrl)
        {
            Content = JsonContent.Create(ghtkBody)
        };
        httpRequest.Headers.Add("Token", _ghtk.Token);

        var response = await client.SendAsync(httpRequest);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return BadRequest(new { error = "GHTK API failed", detail = json });

        var ghtkResult = JsonSerializer.Deserialize<GhtkResponse<GhtkCreateOrderData>>(json);
        if (ghtkResult is null || !ghtkResult.Success || ghtkResult.Order is null)
            return BadRequest(new { error = ghtkResult?.Message ?? "Unknown error", detail = json });

        // Lưu order vào DB
        var order = new Order
        {
            Id = Guid.NewGuid(),
            ToName = request.Name,
            ToPhone = request.Tel,
            ToAddress = request.Address,
            Weight = request.Weight,
            CodAmount = request.PickMoney,
            InsuranceValue = request.Value,
            Provider = ShippingProvider.GHTK,
            GhtkLabel = ghtkResult.Order.Label,
            GhtkTrackingId = ghtkResult.Order.TrackingId,
            ShippingFee = decimal.TryParse(ghtkResult.Order.Fee, out var fee) ? fee : 0,
            Status = ShippingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            order.Id,
            order.GhtkLabel,
            order.GhtkTrackingId,
            order.ShippingFee,
            ghtkResult.Order.EstimatedPickTime,
            ghtkResult.Order.EstimatedDeliverTime,
            order.Status
        });
    }

    // ──────────────────────────────────────────────
    // 2. GET /api/shipping/ghtk/calculate-fee  – Tính phí ship GHTK
    // ──────────────────────────────────────────────
    [HttpGet("calculate-fee")]
    public async Task<IActionResult> CalculateFee([FromQuery] GhtkCalculateFeeRequest request)
    {
        var client = _httpClientFactory.CreateClient("GHTK");

        // GHTK dùng GET + query string
        var queryParams = new Dictionary<string, string?>
        {
            ["pick_province"] = request.PickProvince,
            ["pick_district"] = request.PickDistrict,
            ["province"] = request.Province,
            ["district"] = request.District,
            ["address"] = request.Address,
            ["weight"] = request.Weight.ToString(),
            ["value"] = request.Value.ToString(),
            ["transport"] = request.Transport
        };

        var queryString = string.Join("&",
            queryParams.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value ?? "")}"));

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{_ghtk.CalculateFeeUrl}?{queryString}");
        httpRequest.Headers.Add("Token", _ghtk.Token);

        var response = await client.SendAsync(httpRequest);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return BadRequest(new { error = "GHTK API failed", detail = json });

        var ghtkResult = JsonSerializer.Deserialize<GhtkFeeResponse>(json);
        if (ghtkResult is null || !ghtkResult.Success || ghtkResult.Fee is null)
            return BadRequest(new { error = ghtkResult?.Message ?? "Unknown error", detail = json });

        return Ok(ghtkResult.Fee);
    }

    // ──────────────────────────────────────────────
    // 3. GET /api/shipping/ghtk/orders/{id}/tracking  – Theo dõi trạng thái
    // ──────────────────────────────────────────────
    [HttpGet("orders/{id:guid}/tracking")]
    public async Task<IActionResult> GetTracking(Guid id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order is null)
            return NotFound(new { error = "Order not found" });

        if (string.IsNullOrEmpty(order.GhtkLabel))
            return BadRequest(new { error = "Order has no GHTK tracking label" });

        var client = _httpClientFactory.CreateClient("GHTK");

        var httpRequest = new HttpRequestMessage(HttpMethod.Get,
            $"{_ghtk.TrackingBaseUrl}/{order.GhtkLabel}");
        httpRequest.Headers.Add("Token", _ghtk.Token);

        var response = await client.SendAsync(httpRequest);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return BadRequest(new { error = "GHTK API failed", detail = json });

        var ghtkResult = JsonSerializer.Deserialize<GhtkResponse<GhtkTrackingData>>(json);
        if (ghtkResult is null || !ghtkResult.Success || ghtkResult.Order is null)
            return BadRequest(new { error = ghtkResult?.Message ?? "Unknown error", detail = json });

        return Ok(new
        {
            order.Id,
            order.GhtkLabel,
            ghtkResult.Order.Status,
            ghtkResult.Order.StatusText,
            ghtkResult.Order.PickDate,
            ghtkResult.Order.DeliverDate
        });
    }
}
