namespace Crm.Application.DealItems.DTOs;

/// <summary>
/// Запрос на создание позиции сделки.
/// </summary>
public sealed class CreateDealItemRequest
{
    /// <summary>
    /// Идентификатор товара.
    /// </summary>
    public Guid ProductId { get; init; }

    /// <summary>
    /// Количество товара.
    /// </summary>
    public decimal Quantity { get; init; }

    /// <summary>
    /// Цена за единицу товара.
    /// </summary>
    public decimal UnitPrice { get; init; }
}