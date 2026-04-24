using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models;

public class Room
{
    public int Id { get; set; }

    public string RoomNumber { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty; // Deluxe, Suite, Standard

    public decimal PricePerNight { get; set; }

    public int MaxOccupancy { get; set; }

    public int HotelId { get; set; }

    [ForeignKey("HotelId")]
    public Hotel Hotel { get; set; } = null!;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}