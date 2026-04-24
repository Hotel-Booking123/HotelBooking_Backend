

using HotelBooking.DTOs;

namespace HotelBooking.Services.Interfaces;

public interface IBookingService
{
    Task<BookingResponse> CreateBookingAsync(int userId, BookingRequest request);
    Task<List<BookingResponse>> GetUserBookingsAsync(int userId);
    Task<List<BookingResponse>> GetAllBookingsAsync();
    Task<BookingResponse?> GetBookingByIdAsync(int bookingId);
    Task<BookingResponse> RebookBookingAsync(int userId, int oldBookingId);
    Task<bool> CancelBookingAsync(int bookingId, int userId);
}
