using Crm.Application.Products.DTOs;

namespace Crm.Application.Products;

/// <summary>
/// Сервис управления товарами.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Создает новый товар.
    /// </summary>
    Task<Guid> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает товар по идентификатору.
    /// </summary>
    Task<ProductResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает список товаров с учетом фильтров.
    /// </summary>
    Task<IReadOnlyCollection<ProductListItemResponse>> GetAllAsync(
        GetProductsRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет данные товара.
    /// </summary>
    Task UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Деактивирует товар.
    /// </summary>
    Task DeactivateAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Активирует товар.
    /// </summary>
    Task ActivateAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет товар.
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}