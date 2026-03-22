using Crm.Application.DealItems.DTOs;

namespace Crm.Application.DealItems;

/// <summary>
/// Сервис управления позициями сделок.
/// </summary>
public interface IDealItemService
{
    /// <summary>
    /// Создает новую позицию сделки.
    /// </summary>
    Task<Guid> CreateAsync(Guid dealId, CreateDealItemRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает позицию сделки по идентификатору.
    /// </summary>
    Task<DealItemResponse> GetByIdAsync(Guid dealId, Guid itemId, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает список позиций сделки.
    /// </summary>
    Task<IReadOnlyCollection<DealItemListItemResponse>> GetAllAsync(Guid dealId, CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет позицию сделки.
    /// </summary>
    Task UpdateAsync(Guid dealId, Guid itemId, UpdateDealItemRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет позицию сделки.
    /// </summary>
    Task DeleteAsync(Guid dealId, Guid itemId, CancellationToken cancellationToken);
}