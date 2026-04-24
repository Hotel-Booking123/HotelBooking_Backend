public interface IHotelService
{
    Task<List<HotelResponse>> GetAllHotelsAsync();
    Task<HotelResponse?> GetHotelByIdAsync(int id);
    Task<List<HotelResponse>> SearchHotelsAsync(HotelSearchQuery query);
    Task<HotelResponse> CreateHotelAsync(CreateHotelRequest request);
    Task<HotelResponse?> UpdateHotelAsync(int id, UpdateHotelRequest request);
    Task<bool> DeleteHotelAsync(int id);
}
