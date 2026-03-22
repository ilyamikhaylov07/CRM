using Crm.Application.Deals.DTOs;

namespace Crm.Application.Deals;

/// <summary>
/// Сервис управления сделками.
/// </summary>
public interface IDealService
{
    /// <summary>
    /// Создает новую сделку.
    /// </summary>
    Task<Guid> CreateAsync(CreateDealRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает сделку по идентификатору.
    /// </summary>
    Task<DealResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает список сделок с учетом фильтров.
    /// </summary>
    Task<IReadOnlyCollection<DealListItemResponse>> GetAllAsync(
        GetDealsRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет данные сделки.
    /// </summary>
    Task UpdateAsync(Guid id, UpdateDealRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет сделку.
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}