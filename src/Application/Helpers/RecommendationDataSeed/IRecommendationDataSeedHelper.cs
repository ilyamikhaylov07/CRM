using Crm.Application.Helpers.RecommendationDataSeed.DTOs;

namespace Crm.Application.Helpers.RecommendationDataSeed;

/// <summary>
/// Helper-сервис для одноразового наполнения истории покупок под обучение рекомендательной модели.
/// </summary>
public interface IRecommendationDataSeedHelper
{
    /// <summary>
    /// Генерирует дополнительные сделки и позиции сделок на основе существующих клиентов и товаров.
    /// </summary>
    /// <param name="request">Параметры генерации тестовых данных.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат генерации данных.</returns>
    Task<SeedRecommendationDataResponse> SeedAsync(
        SeedRecommendationDataRequest request,
        CancellationToken cancellationToken);
}
