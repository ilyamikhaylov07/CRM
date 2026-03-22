namespace Crm.Application.DealItems.DTOs;

/// <summary>
/// Краткая информация о позиции сделки.
/// </summary>
public sealed class DealItemListItemResponse
{
    /// <summary>
    /// Идентификатор позиции сделки.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Идентификатор товара.
    /// </summary>
    public Guid ProductId { get; init; }

    /// <summary>
    /// Наименование товара.
    /// </summary>
    public required string ProductName { get; init; }

    /// <summary>
    /// Количество товара.
    /// </summary>
    public decimal Quantity { get; init; }

    /// <summary>
    /// Цена за единицу товара.
    /// </summary>
    public decimal UnitPrice { get; init; }

    /// <summary>
    /// Итоговая стоимость позиции.
    /// </summary>
    public decimal TotalPrice { get; init; }
}