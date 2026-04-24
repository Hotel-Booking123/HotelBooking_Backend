using HotelBooking.DTOs;

namespace HotelBooking.Services.Interfaces;

public interface IPromotionService
{
    Task<PromotionResponse?> GetPromotionByCodeAsync(string code);
    Task<DiscountResult> ApplyDiscountAsync(string code, decimal subtotal);
    Task<List<PromotionResponse>> GetAllActivePromotionsAsync();
    Task<PromotionResponse> CreatePromotionAsync(CreatePromotionRequest request);
    Task<PromotionResponse?> UpdatePromotionAsync(int id, UpdatePromotionRequest request);
    Task<bool> DeletePromotionAsync(int id);
}
