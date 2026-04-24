namespace HotelBooking.DTOs;


public class BookingRequest
{
    public int RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string? DiscountCode { get; set; }
}

public class BookingResponse
{
    public int Id { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public string BookingReference { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }

    // ✅ Add these for admin view
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
}
