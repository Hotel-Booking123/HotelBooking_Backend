namespace HotelBooking.Models;
public class Booking
{
    public int Id { get; set; }

    public DateTime CheckInDate { get; set; }

    public DateTime CheckOutDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = "Confirmed";

    public string? DiscountCode { get; set; }

    public DateTime BookingDate { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
}