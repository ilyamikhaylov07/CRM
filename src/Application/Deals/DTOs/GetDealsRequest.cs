namespace Crm.Application.Deals.DTOs;

/// <summary>
/// Параметры запроса списка сделок.
/// </summary>
public sealed class GetDealsRequest
{
    /// <summary>
    /// Фильтр по клиенту.
    /// </summary>
    public Guid? ClientId { get; init; }

    /// <summary>
    /// Фильтр по пользователю.
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Фильтр по способу оплаты.
    /// </summary>
    public string? PaymentMethod { get; init; }

    /// <summary>
    /// Фильтр по сезону.
    /// </summary>
    public string? Season { get; init; }

    /// <summary>
    /// Минимальная дата покупки.
    /// </summary>
    public DateTime? PurchaseDateFromUtc { get; init; }

    /// <summary>
    /// Максимальная дата покупки.
    /// </summary>
    public DateTime? PurchaseDateToUtc { get; init; }

    /// <summary>
    /// Минимальная сумма покупки.
    /// </summary>
    public decimal? MinPurchaseAmount { get; init; }

    /// <summary>
    /// Максимальная сумма покупки.
    /// </summary>
    public decimal? MaxPurchaseAmount { get; init; }
}