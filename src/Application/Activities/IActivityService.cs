using Crm.Application.Activities.DTOs;

namespace Crm.Application.Activities;

/// <summary>
/// Сервис управления активностями.
/// </summary>
public interface IActivityService
{
    /// <summary>
    /// Создает новую активность.
    /// </summary>
    Task<Guid> CreateAsync(CreateActivityRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает активность по идентификатору.
    /// </summary>
    Task<ActivityResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает список активностей с учетом фильтров.
    /// </summary>
    Task<IReadOnlyCollection<ActivityListItemResponse>> GetAllAsync(
        GetActivitiesRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет активность.
    /// </summary>
    Task UpdateAsync(Guid id, UpdateActivityRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет активность.
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}