using HotelBooking.Data;
using HotelBooking.DTOs;
using HotelBooking.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Services.Implementations;

public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _context;
    private readonly IPromotionService _promotionService;
    private readonly IEmailService _emailService;
    private readonly IRoomService _roomService;

    public BookingService(ApplicationDbContext context, IPromotionService promotionService, IEmailService emailService, IRoomService roomService)
    {
        _context = context;
        _promotionService = promotionService;
        _emailService = emailService;
        _roomService = roomService;
    }

    public async Task<BookingResponse> CreateBookingAsync(int userId, BookingRequest request)
    {
        if (request.CheckInDate >= request.CheckOutDate)
            throw new Exception("Check-out must be after check-in");

        if (!await _roomService.IsRoomAvailableAsync(request.RoomId, request.CheckInDate, request.CheckOutDate))
            throw new Exception("Room not available for selected dates");

        var room = await _context.Rooms.Include(r => r.Hotel).FirstOrDefaultAsync(r => r.Id == request.RoomId);
        if (room == null) throw new Exception("Room not found");

        int nights = (int)(request.CheckOutDate - request.CheckInDate).TotalDays;
        decimal subtotal = room.PricePerNight * nights;

        decimal discountAmount = 0;
        if (!string.IsNullOrEmpty(request.DiscountCode))
        {
            var discountResult = await _promotionService.ApplyDiscountAsync(request.DiscountCode, subtotal);
            if (discountResult.IsValid)
                discountAmount = discountResult.DiscountAmount;
            else
                throw new Exception(discountResult.Message);
        }

        decimal total = subtotal - discountAmount;

        var booking = new Booking
        {
            RoomId = request.RoomId,
            UserId = userId,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            TotalPrice = total,
            DiscountCode = request.DiscountCode,
            Status = "Confirmed",
            BookingDate = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Load user and room details for response
        var user = await _context.Users.FindAsync(userId);
        await _emailService.SendBookingConfirmationAsync(user!.Email, booking.Id, room.Hotel.Name, room.RoomNumber, request.CheckInDate, request.CheckOutDate, total);

        return new BookingResponse
        {
            Id = booking.Id,
            HotelName = room.Hotel.Name,
            RoomNumber = room.RoomNumber,
            RoomType = room.Type,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status,
            BookingReference = $"BK-{booking.Id}",
            BookingDate = booking.BookingDate
        };
    }

    public async Task<List<BookingResponse>> GetUserBookingsAsync(int userId)
    {
        var bookings = await _context.Bookings
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();

        return bookings.Select(b => new BookingResponse
        {
            Id = b.Id,
            HotelName = b.Room?.Hotel?.Name ?? "Unknown Hotel",
            RoomNumber = b.Room?.RoomNumber ?? "?",
            RoomType = b.Room?.Type ?? "?",
            CheckInDate = b.CheckInDate,
            CheckOutDate = b.CheckOutDate,
            TotalPrice = b.TotalPrice,
            Status = b.Status,
            BookingReference = $"BK-{b.Id}",
            BookingDate = b.BookingDate
            // No user fields for regular users (for privacy)
        }).ToList();
    }

    public async Task<BookingResponse?> GetBookingByIdAsync(int bookingId)
    {
        var booking = await _context.Bookings
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null) return null;

        return new BookingResponse
        {
            Id = booking.Id,
            HotelName = booking.Room.Hotel.Name,
            RoomNumber = booking.Room.RoomNumber,
            RoomType = booking.Room.Type,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status,
            BookingReference = $"BK-{booking.Id}",
            BookingDate = booking.BookingDate
        };
    }

    public async Task<BookingResponse> RebookBookingAsync(int userId, int oldBookingId)
    {
        var oldBooking = await GetBookingByIdAsync(oldBookingId);
        if (oldBooking == null) throw new Exception("Original booking not found");

        // Create new booking for same room, default dates +1 day to +3 days
        var newRequest = new BookingRequest
        {
            RoomId = (await _context.Bookings.FindAsync(oldBookingId))!.RoomId,
            CheckInDate = DateTime.UtcNow.AddDays(1),
            CheckOutDate = DateTime.UtcNow.AddDays(3),
            DiscountCode = null
        };

        return await CreateBookingAsync(userId, newRequest);
    }

    public async Task<List<BookingResponse>> GetAllBookingsAsync()
    {
        var bookings = await _context.Bookings
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .Include(b => b.User)  // ✅ Include User data
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();

        return bookings.Select(b => new BookingResponse
        {
            Id = b.Id,
            HotelName = b.Room?.Hotel?.Name ?? "Unknown Hotel",
            RoomNumber = b.Room?.RoomNumber ?? "?",
            RoomType = b.Room?.Type ?? "?",
            CheckInDate = b.CheckInDate,
            CheckOutDate = b.CheckOutDate,
            TotalPrice = b.TotalPrice,
            Status = b.Status,
            BookingReference = $"BK-{b.Id}",
            BookingDate = b.BookingDate,
            // ✅ Include user info
            UserName = b.User?.FullName,
            UserEmail = b.User?.Email
        }).ToList();
    }

    public async Task<bool> CancelBookingAsync(int bookingId, int userId)
    {
        var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);
        if (booking == null) return false;

        booking.Status = "Cancelled";
        await _context.SaveChangesAsync();

        var user = await _context.Users.FindAsync(userId);
        await _emailService.SendBookingCancellationAsync(user!.Email, bookingId);
        return true;
    }
}
