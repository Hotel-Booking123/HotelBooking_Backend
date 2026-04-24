using System.ComponentModel.DataAnnotations;
namespace HotelBooking.DTOs;

public class RoomResponse
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int MaxOccupancy { get; set; }
    public bool IsAvailable { get; set; }
}

public class AvailabilityRequest
{
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
}

public class CreateRoomRequest
{
    [Required] public string RoomNumber { get; set; } = string.Empty;
    [Required] public string Type { get; set; } = string.Empty;
    [Required] public decimal PricePerNight { get; set; }
    [Required] public int MaxOccupancy { get; set; }
}

public class UpdateRoomRequest
{
    public string? RoomNumber { get; set; }
    public string? Type { get; set; }
    public decimal? PricePerNight { get; set; }
    public int? MaxOccupancy { get; set; }
}