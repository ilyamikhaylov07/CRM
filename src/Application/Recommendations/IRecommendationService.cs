using Crm.Application.Recommendations.DTOs;

namespace Crm.Application.Recommendations;

/// <summary>
/// Сервис работы с сохранёнными рекомендациями.
/// </summary>
public interface IRecommendationService
{
    /// <summary>
    /// Возвращает список рекомендаций с учетом фильтров.
    /// </summary>
    /// <param name="request">Параметры фильтрации списка рекомендаций.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция рекомендаций.</returns>
    Task<IReadOnlyCollection<RecommendationListItemResponse>> GetAllAsync(
        GetRecommendationsRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает рекомендацию по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор рекомендации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные рекомендации.</returns>
    Task<RecommendationResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает рекомендации конкретного клиента.
    /// </summary>
    /// <param name="clientId">Идентификатор клиента.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция рекомендаций клиента.</returns>
    Task<IReadOnlyCollection<RecommendationListItemResponse>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет статус рекомендации.
    /// </summary>
    /// <param name="id">Идентификатор рекомендации.</param>
    /// <param name="request">Новый статус рекомендации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task UpdateStatusAsync(Guid id, ChangeRecommendationStatusRequest request, CancellationToken cancellationToken);
}
