namespace HotelBooking.Models;

public class Promotion
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string DiscountType { get; set; } = "Percentage"; // Percentage or Fixed

    public decimal DiscountValue { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public bool IsActive { get; set; } = true;
}