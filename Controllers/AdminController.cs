using HotelBooking.Services.Interfaces;
using HotelBooking.DTOs;
using HotelBooking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly IRoomService _roomService;
    private readonly IPromotionService _promotionService;
    private readonly IUserService _userService;

    public AdminController(IHotelService hotelService, IRoomService roomService, IPromotionService promotionService, IUserService userService)
    {
        _hotelService = hotelService;
        _roomService = roomService;
        _promotionService = promotionService;
        _userService = userService;
    }

    // Hotel Management
    [HttpPost("hotels")]
    public async Task<IActionResult> CreateHotel(CreateHotelRequest request)
        => Ok(await _hotelService.CreateHotelAsync(request));

    [HttpPut("hotels/{id}")]
    public async Task<IActionResult> UpdateHotel(int id, UpdateHotelRequest request)
    {
        var result = await _hotelService.UpdateHotelAsync(id, request);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("hotels/{id}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        var result = await _hotelService.DeleteHotelAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Hotel deleted" });
    }

    // Room Management
    // ✅ Fixed: hotelId is required in the route
    [HttpPost("hotels/{hotelId}/rooms")]
    public async Task<IActionResult> CreateRoom(int hotelId, [FromBody] CreateRoomRequest request)
        => Ok(await _roomService.CreateRoomAsync(hotelId, request));

    [HttpPut("rooms/{id}")]
    public async Task<IActionResult> UpdateRoom(int id, [FromBody] UpdateRoomRequest request)
    {
        var result = await _roomService.UpdateRoomAsync(id, request);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("rooms/{id}")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var result = await _roomService.DeleteRoomAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Room deleted" });
    }

    // Promotion Management
    [HttpPost("promotions")]
    public async Task<IActionResult> CreatePromotion(CreatePromotionRequest request)
        => Ok(await _promotionService.CreatePromotionAsync(request));

    [HttpPut("promotions/{id}")]
    public async Task<IActionResult> UpdatePromotion(int id, UpdatePromotionRequest request)
    {
        var result = await _promotionService.UpdatePromotionAsync(id, request);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("promotions/{id}")]
    public async Task<IActionResult> DeletePromotion(int id)
    {
        var result = await _promotionService.DeletePromotionAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Promotion deleted" });
    }

    // User Management
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
        => Ok(await _userService.GetAllUsersAsync());

    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        try
        {
            var user = await _userService.CreateUserAsync(request);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
    {
        var result = await _userService.UpdateUserAsync(id, request);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "User deleted" });
    }
}