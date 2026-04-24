public interface IRoomService
{
    Task<List<RoomResponse>> GetRoomsByHotelIdAsync(int hotelId, AvailabilityRequest? availability = null);
    Task<RoomResponse?> GetRoomByIdAsync(int id);
    Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut);
    Task<RoomResponse> CreateRoomAsync(int hotelId, CreateRoomRequest request);
    Task<RoomResponse?> UpdateRoomAsync(int id, UpdateRoomRequest request);
    Task<bool> DeleteRoomAsync(int id);
}