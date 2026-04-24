using System.ComponentModel.DataAnnotations;


namespace HotelBooking.Models;

public class Hotel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Location { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Amenities { get; set; } = string.Empty; // comma-separated

    public double AverageRating { get; set; }

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}