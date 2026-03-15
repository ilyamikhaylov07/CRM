namespace Crm.Infrastructure.Import;

public sealed class ShoppingTrendRuRow
{
    public string CustomerId { get; set; } = null!;
    public int Age { get; set; }
    public string Gender { get; set; } = null!;
    public string ItemPurchased { get; set; } = null!;
    public string Category { get; set; } = null!;
    public decimal PurchaseAmountUsd { get; set; }
    public string Location { get; set; } = null!;
    public string Size { get; set; } = null!;
    public string Color { get; set; } = null!;
    public string Season { get; set; } = null!;
    public decimal ReviewRating { get; set; }
    public string SubscriptionStatus { get; set; } = null!;
    public string PaymentMethod { get; set; } = null!;
    public string ShippingType { get; set; } = null!;
    public string DiscountApplied { get; set; } = null!;
    public string PromoCodeUsed { get; set; } = null!;
    public int PreviousPurchases { get; set; }
    public string PreferredPaymentMethod { get; set; } = null!;
    public string FrequencyOfPurchases { get; set; } = null!;
}
