using HotelBooking.Data;
using HotelBooking.DTOs;
using HotelBooking.Models;
using HotelBooking.Services.Interfaces;
using HotelBooking.Data;
using HotelBooking.DTOs;
using HotelBooking.Models;
using HotelBooking.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Services.Implementations;

public class PromotionService : IPromotionService
{
    private readonly ApplicationDbContext _context;

    public PromotionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PromotionResponse?> GetPromotionByCodeAsync(string code)
    {
        var promo = await _context.Promotions
            .FirstOrDefaultAsync(p => p.Code == code && p.IsActive && p.ValidFrom <= DateTime.UtcNow && p.ValidTo >= DateTime.UtcNow);

        if (promo == null) return null;

        return new PromotionResponse
        {
            Id = promo.Id,
            Code = promo.Code,
            DiscountType = promo.DiscountType,
            DiscountValue = promo.DiscountValue,
            ValidFrom = promo.ValidFrom,
            ValidTo = promo.ValidTo,
            IsActive = promo.IsActive
        };
    }

    public async Task<DiscountResult> ApplyDiscountAsync(string code, decimal subtotal)
    {
        var promo = await GetPromotionByCodeAsync(code);
        if (promo == null)
            return new DiscountResult { IsValid = false, Message = "Invalid or expired discount code" };

        decimal discountAmount = 0;
        if (promo.DiscountType == "Percentage")
        {
            discountAmount = subtotal * (promo.DiscountValue / 100);
        }
        else if (promo.DiscountType == "Fixed")
        {
            discountAmount = promo.DiscountValue;
            if (discountAmount > subtotal) discountAmount = subtotal;
        }

        return new DiscountResult { IsValid = true, DiscountAmount = discountAmount };
    }

    public async Task<List<PromotionResponse>> GetAllActivePromotionsAsync()
    {
        var promos = await _context.Promotions
            .Where(p => p.IsActive && p.ValidFrom <= DateTime.UtcNow && p.ValidTo >= DateTime.UtcNow)
            .ToListAsync();

        return promos.Select(p => new PromotionResponse
        {
            Id = p.Id,
            Code = p.Code,
            DiscountType = p.DiscountType,
            DiscountValue = p.DiscountValue,
            ValidFrom = p.ValidFrom,
            ValidTo = p.ValidTo,
            IsActive = p.IsActive
        }).ToList();
    }

    public async Task<PromotionResponse> CreatePromotionAsync(CreatePromotionRequest request)
    {
        var promo = new Promotion
        {
            Code = request.Code,
            DiscountType = request.DiscountType,
            DiscountValue = request.DiscountValue,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            IsActive = request.IsActive
        };
        _context.Promotions.Add(promo);
        await _context.SaveChangesAsync();

        return new PromotionResponse
        {
            Id = promo.Id,
            Code = promo.Code,
            DiscountType = promo.DiscountType,
            DiscountValue = promo.DiscountValue,
            ValidFrom = promo.ValidFrom,
            ValidTo = promo.ValidTo,
            IsActive = promo.IsActive
        };
    }

    public async Task<PromotionResponse?> UpdatePromotionAsync(int id, UpdatePromotionRequest request)
    {
        var promo = await _context.Promotions.FindAsync(id);
        if (promo == null) return null;

        if (!string.IsNullOrEmpty(request.Code)) promo.Code = request.Code;
        if (!string.IsNullOrEmpty(request.DiscountType)) promo.DiscountType = request.DiscountType;
        if (request.DiscountValue.HasValue) promo.DiscountValue = request.DiscountValue.Value;
        if (request.ValidFrom.HasValue) promo.ValidFrom = request.ValidFrom.Value;
        if (request.ValidTo.HasValue) promo.ValidTo = request.ValidTo.Value;
        if (request.IsActive.HasValue) promo.IsActive = request.IsActive.Value;

        await _context.SaveChangesAsync();

        return new PromotionResponse
        {
            Id = promo.Id,
            Code = promo.Code,
            DiscountType = promo.DiscountType,
            DiscountValue = promo.DiscountValue,
            ValidFrom = promo.ValidFrom,
            ValidTo = promo.ValidTo,
            IsActive = promo.IsActive
        };
    }

    public async Task<bool> DeletePromotionAsync(int id)
    {
        var promo = await _context.Promotions.FindAsync(id);
        if (promo == null) return false;
        _context.Promotions.Remove(promo);
        await _context.SaveChangesAsync();
        return true;
    }
}