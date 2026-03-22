namespace Crm.Application.Products.DTOs;

/// <summary>
/// Параметры запроса списка товаров.
/// </summary>
public sealed class GetProductsRequest
{
    /// <summary>
    /// Поисковая строка по имени, категории, цвету или размеру.
    /// </summary>
    public string? Search { get; init; }

    /// <summary>
    /// Фильтр по категории.
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Фильтр по признаку активности.
    /// </summary>
    public bool? IsActive { get; init; }

    /// <summary>
    /// Минимальная цена.
    /// </summary>
    public decimal? MinPrice { get; init; }

    /// <summary>
    /// Максимальная цена.
    /// </summary>
    public decimal? MaxPrice { get; init; }
}