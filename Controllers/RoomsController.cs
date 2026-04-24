using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/hotels/{hotelId}/rooms")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRooms(int hotelId, [FromQuery] AvailabilityRequest? availability)
    {
        var rooms = await _roomService.GetRoomsByHotelIdAsync(hotelId, availability);
        return Ok(rooms);
    }

    [HttpGet("{roomId}")]
    public async Task<IActionResult> GetRoom(int hotelId, int roomId)
    {
        var room = await _roomService.GetRoomByIdAsync(roomId);
        if (room == null || room.Id != roomId) return NotFound();
        return Ok(room);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateRoom(int hotelId, [FromBody] CreateRoomRequest request)
    {
        try
        {
            var room = await _roomService.CreateRoomAsync(hotelId, request);
            return Ok(room);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{roomId}")]
    public async Task<IActionResult> UpdateRoom(int hotelId, int roomId, [FromBody] UpdateRoomRequest request)
    {
        try
        {
            var result = await _roomService.UpdateRoomAsync(roomId, request);
            if (result == null) return NotFound();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{roomId}")]
    public async Task<IActionResult> DeleteRoom(int hotelId, int roomId)
    {
        var result = await _roomService.DeleteRoomAsync(roomId);
        if (!result) return NotFound();
        return Ok(new { message = "Room deleted" });
    }
}