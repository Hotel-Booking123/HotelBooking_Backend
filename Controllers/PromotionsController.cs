using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelBooking.DTOs;
using HotelBooking.Services.Interfaces;
using HotelBooking.Services.Interfaces;

namespace HotelBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromotionsController : ControllerBase
{
    private readonly IPromotionService _promotionService;

    public PromotionsController(IPromotionService promotionService)
    {
        _promotionService = promotionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllActive()
    {
        var promos = await _promotionService.GetAllActivePromotionsAsync();
        return Ok(promos);
    }

    [HttpGet("validate")]
    public async Task<IActionResult> Validate([FromQuery] string code, [FromQuery] decimal subtotal)
    {
        var result = await _promotionService.ApplyDiscountAsync(code, subtotal);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreatePromotion(CreatePromotionRequest request)
    {
        var promo = await _promotionService.CreatePromotionAsync(request);
        return Ok(promo);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePromotion(int id, UpdatePromotionRequest request)
    {
        var result = await _promotionService.UpdatePromotionAsync(id, request);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePromotion(int id)
    {
        var result = await _promotionService.DeletePromotionAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Promotion deleted" });
    }
}