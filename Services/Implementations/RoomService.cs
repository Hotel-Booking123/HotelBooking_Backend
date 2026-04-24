
public class RoomService : IRoomService
{
    private readonly ApplicationDbContext _context;

    public RoomService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoomResponse>> GetRoomsByHotelIdAsync(int hotelId, AvailabilityRequest? availability = null)
    {
        var roomsQuery = _context.Rooms.Where(r => r.HotelId == hotelId);

        if (availability != null && availability.CheckIn < availability.CheckOut)
        {
            var availableRoomIds = await _context.Rooms
                .Where(r => r.HotelId == hotelId && !_context.Bookings.Any(b =>
                    b.RoomId == r.Id &&
                    b.Status == "Confirmed" &&
                    b.CheckInDate < availability.CheckOut &&
                    b.CheckOutDate > availability.CheckIn))
                .Select(r => r.Id)
                .ToListAsync();

            roomsQuery = roomsQuery.Where(r => availableRoomIds.Contains(r.Id));
        }

        var rooms = await roomsQuery.ToListAsync();

        return rooms.Select(r => new RoomResponse
        {
            Id = r.Id,
            RoomNumber = r.RoomNumber,
            Type = r.Type,
            PricePerNight = r.PricePerNight,
            MaxOccupancy = r.MaxOccupancy,
            IsAvailable = availability == null || IsRoomAvailableAsync(r.Id, availability!.CheckIn, availability.CheckOut).Result
        }).ToList();
    }

    public async Task<RoomResponse?> GetRoomByIdAsync(int id)
    {
        var room = await _context.Rooms.Include(r => r.Hotel).FirstOrDefaultAsync(r => r.Id == id);
        if (room == null) return null;

        return new RoomResponse
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            Type = room.Type,
            PricePerNight = room.PricePerNight,
            MaxOccupancy = room.MaxOccupancy,
            IsAvailable = true
        };
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        return !await _context.Bookings.AnyAsync(b =>
            b.RoomId == roomId &&
            b.Status == "Confirmed" &&
            b.CheckInDate < checkOut &&
            b.CheckOutDate > checkIn);
    }

    public async Task<RoomResponse> CreateRoomAsync(int hotelId, CreateRoomRequest request)
    {
        // Verify hotel exists
        var hotel = await _context.Hotels.FindAsync(hotelId);
        if (hotel == null)
            throw new Exception("Hotel not found");

        var room = new Room
        {
            RoomNumber = request.RoomNumber,
            Type = request.Type,
            PricePerNight = request.PricePerNight,
            MaxOccupancy = request.MaxOccupancy,
            HotelId = hotelId
        };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return new RoomResponse
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            Type = room.Type,
            PricePerNight = room.PricePerNight,
            MaxOccupancy = room.MaxOccupancy,
            IsAvailable = true
        };
    }

    public async Task<RoomResponse?> UpdateRoomAsync(int id, UpdateRoomRequest request)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return null;

        if (!string.IsNullOrEmpty(request.RoomNumber)) room.RoomNumber = request.RoomNumber;
        if (!string.IsNullOrEmpty(request.Type)) room.Type = request.Type;
        if (request.PricePerNight.HasValue) room.PricePerNight = request.PricePerNight.Value;
        if (request.MaxOccupancy.HasValue) room.MaxOccupancy = request.MaxOccupancy.Value;

        await _context.SaveChangesAsync();

        return new RoomResponse
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            Type = room.Type,
            PricePerNight = room.PricePerNight,
            MaxOccupancy = room.MaxOccupancy,
            IsAvailable = true
        };
    }

    public async Task<bool> DeleteRoomAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return false;
        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
        return true;
    }
}
