using Crm.Application.ML.DTOs;

namespace Crm.Application.ML.Recommendations;

/// <summary>
/// Определяет контракт сервиса для генерации и сохранения рекомендаций товаров.
/// </summary>
public interface IRecommendationEngineService
{
    /// <summary>
    /// Генерирует рекомендации для клиентов и сохраняет их в системе.
    /// </summary>
    /// <param name="topPerClient">Количество рекомендаций на одного клиента.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция сгенерированных рекомендаций.</returns>
    Task<IReadOnlyCollection<RecommendationResultResponse>> GenerateAndStoreAsync(int topPerClient, CancellationToken cancellationToken);
}
