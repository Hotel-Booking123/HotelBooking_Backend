using System.ComponentModel.DataAnnotations;

namespace HotelBooking.DTOs;


public class HotelResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Amenities { get; set; } = string.Empty;
    public double AverageRating { get; set; }
}

public class HotelSearchQuery
{
    public string? Location { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Amenities { get; set; }
}

public class CreateHotelRequest
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Amenities { get; set; } = string.Empty;
    public double AverageRating { get; set; }
}

public class UpdateHotelRequest
{
    public string? Name { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public string? Amenities { get; set; }
    public double? AverageRating { get; set; }
}using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.DTOs;

public class HotelResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Amenities { get; set; } = string.Empty;
    public double AverageRating { get; set; }
}

public class HotelSearchQuery
{
    public string? Location { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Amenities { get; set; }
}

public class CreateHotelRequest
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Amenities { get; set; } = string.Empty;
    public double AverageRating { get; set; }
}

public class UpdateHotelRequest
{
    public string? Name { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public string? Amenities { get; set; }
    public double? AverageRating { get; set; }
}