namespace Crm.Application.Products.DTOs;

/// <summary>
/// Запрос на создание товара.
/// </summary>
public sealed class CreateProductRequest
{
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
}