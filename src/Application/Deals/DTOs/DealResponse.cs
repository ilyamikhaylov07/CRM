namespace Crm.Application.Deals.DTOs;

/// <summary>
/// Полная информация о сделке.
/// </summary>
public sealed class DealResponse
{
    /// <summary>
    /// Идентификатор сделки.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Идентификатор клиента.
    /// </summary>
    public Guid ClientId { get; init; }

    /// <summary>
    /// Имя клиента.
    /// </summary>
    public required string ClientName { get; init; }

    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string? UserFullName { get; init; }

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