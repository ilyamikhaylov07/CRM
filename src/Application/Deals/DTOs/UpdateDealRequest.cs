namespace Crm.Application.Deals.DTOs;

/// <summary>
/// Запрос на обновление сделки.
/// </summary>
public sealed class UpdateDealRequest
{
    /// <summary>
    /// Идентификатор пользователя, ответственного за сделку.
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Сумма покупки.
    /// </summary>
    public decimal PurchaseAmount { get; init; }

    /// <summary>
    /// Дата покупки в UTC.
    /// </summary>
    public DateTime PurchaseDateUtc { get; init; }

    /// <summary>
    /// Сезон покупки.
    /// </summary>
    public required string Season { get; init; }

    /// <summary>
    /// Способ оплаты.
    /// </summary>
    public required string PaymentMethod { get; init; }

    /// <summary>
    /// Признак применения скидки.
    /// </summary>
    public bool DiscountApplied { get; init; }

    /// <summary>
    /// Признак использования промокода.
    /// </summary>
    public bool PromoCodeUsed { get; init; }

    /// <summary>
    /// Оценка отзыва.
    /// </summary>
    public decimal ReviewRating { get; init; }

    /// <summary>
    /// Тип доставки.
    /// </summary>
    public string? ShippingType { get; init; }
}