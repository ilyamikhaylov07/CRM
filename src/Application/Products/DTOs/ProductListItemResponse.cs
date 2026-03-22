namespace Crm.Application.Products.DTOs;

/// <summary>
/// Краткая информация о товаре для списков.
/// </summary>
public sealed class ProductListItemResponse
{
    /// <summary>
    /// Идентификатор товара.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Наименование товара.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Категория товара.
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Цвет товара.
    /// </summary>
    public string? Color { get; init; }

    /// <summary>
    /// Размер товара.
    /// </summary>
    public string? Size { get; init; }

    /// <summary>
    /// Базовая цена товара.
    /// </summary>
    public decimal BasePrice { get; init; }

    /// <summary>
    /// Признак активности товара.
    /// </summary>
    public bool IsActive { get; init; }
}