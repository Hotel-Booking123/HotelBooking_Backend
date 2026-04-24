using HotelBooking.Data;
using HotelBooking.DTOs;
using HotelBooking.Models;
using HotelBooking.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Services.Implementations;

public class HotelService : IHotelService
{
    private readonly ApplicationDbContext _context;

    public HotelService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<HotelResponse>> GetAllHotelsAsync()
    {
        return await _context.Hotels
            .Select(h => new HotelResponse
            {
                Id = h.Id,
                Name = h.Name,
                Location = h.Location,
                Description = h.Description,
                Amenities = h.Amenities,
                AverageRating = h.AverageRating
            })
            .ToListAsync();
    }

    public async Task<HotelResponse?> GetHotelByIdAsync(int id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null) return null;

        return new HotelResponse
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Location = hotel.Location,
            Description = hotel.Description,
            Amenities = hotel.Amenities,
            AverageRating = hotel.AverageRating
        };
    }

    public async Task<List<HotelResponse>> SearchHotelsAsync(HotelSearchQuery query)
    {
        var hotelsQuery = _context.Hotels.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Location))
            hotelsQuery = hotelsQuery.Where(h => h.Location.Contains(query.Location));

        if (!string.IsNullOrWhiteSpace(query.Amenities))
        {
            var amenitiesList = query.Amenities.Split(',', StringSplitOptions.RemoveEmptyEntries);
            hotelsQuery = hotelsQuery.Where(h => amenitiesList.Any(a => h.Amenities.Contains(a.Trim())));
        }

        var hotels = await hotelsQuery.ToListAsync();

        var result = new List<HotelResponse>();
        foreach (var hotel in hotels)
        {
            var roomsQuery = _context.Rooms.Where(r => r.HotelId == hotel.Id);

            if (query.MinPrice.HasValue)
                roomsQuery = roomsQuery.Where(r => r.PricePerNight >= query.MinPrice.Value);
            if (query.MaxPrice.HasValue)
                roomsQuery = roomsQuery.Where(r => r.PricePerNight <= query.MaxPrice.Value);

            if (query.CheckIn.HasValue && query.CheckOut.HasValue)
            {
                var availableRoomIds = await _context.Rooms
                    .Where(r => r.HotelId == hotel.Id && !_context.Bookings.Any(b =>
                        b.RoomId == r.Id &&
                        b.Status == "Confirmed" &&
                        b.CheckInDate < query.CheckOut.Value &&
                        b.CheckOutDate > query.CheckIn.Value))
                    .Select(r => r.Id)
                    .ToListAsync();

                roomsQuery = roomsQuery.Where(r => availableRoomIds.Contains(r.Id));
            }

            if (await roomsQuery.AnyAsync())
            {
                result.Add(new HotelResponse
                {
                    Id = hotel.Id,
                    Name = hotel.Name,
                    Location = hotel.Location,
                    Description = hotel.Description,
                    Amenities = hotel.Amenities,
                    AverageRating = hotel.AverageRating
                });
            }
        }

        return result;
    }

    public async Task<HotelResponse> CreateHotelAsync(CreateHotelRequest request)
    {
        var hotel = new Hotel
        {
            Name = request.Name,
            Location = request.Location,
            Description = request.Description,
            Amenities = request.Amenities,
            AverageRating = request.AverageRating
        };
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();
        return new HotelResponse
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Location = hotel.Location,
            Description = hotel.Description,
            Amenities = hotel.Amenities,
            AverageRating = hotel.AverageRating
        };
    }

    public async Task<HotelResponse?> UpdateHotelAsync(int id, UpdateHotelRequest request)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null) return null;

        if (!string.IsNullOrEmpty(request.Name)) hotel.Name = request.Name;
        if (!string.IsNullOrEmpty(request.Location)) hotel.Location = request.Location;
        if (request.Description != null) hotel.Description = request.Description;
        if (request.Amenities != null) hotel.Amenities = request.Amenities;
        if (request.AverageRating.HasValue) hotel.AverageRating = request.AverageRating.Value;

        await _context.SaveChangesAsync();

        return new HotelResponse
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Location = hotel.Location,
            Description = hotel.Description,
            Amenities = hotel.Amenities,
            AverageRating = hotel.AverageRating
        };
    }

    public async Task<bool> DeleteHotelAsync(int id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null) return false;
        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();
        return true;
    }
}
