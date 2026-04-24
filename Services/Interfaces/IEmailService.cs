namespace HotelBooking.Services.Interfaces;

public interface IEmailService
{
    Task SendBookingConfirmationAsync(string toEmail, int bookingId, string hotelName, string roomNumber, DateTime checkIn, DateTime checkOut, decimal totalPrice);
    Task SendBookingCancellationAsync(string toEmail, int bookingId);
}