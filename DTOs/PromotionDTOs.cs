using System.ComponentModel.DataAnnotations;


public class PromotionResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; }
}

public class ApplyDiscountRequest
{
    [Required] public string Code { get; set; } = string.Empty;
    [Required] public decimal Subtotal { get; set; }
}

public class DiscountResult
{
    public bool IsValid { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? Message { get; set; }
}

public class CreatePromotionRequest
{
    [Required] public string Code { get; set; } = string.Empty;
    [Required] public string DiscountType { get; set; } = "Percentage";
    [Required] public decimal DiscountValue { get; set; }
    [Required] public DateTime ValidFrom { get; set; }
    [Required] public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdatePromotionRequest
{
    public string? Code { get; set; }
    public string? DiscountType { get; set; }
    public decimal? DiscountValue { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public bool? IsActive { get; set; }
}