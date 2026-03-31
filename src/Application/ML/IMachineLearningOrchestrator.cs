using Crm.Application.ML.DTOs;

namespace Crm.Application.ML;

/// <summary>
/// Определяет контракт оркестратора для запуска обучения и генерации результатов ML-модулей.
/// </summary>
public interface IMachineLearningOrchestrator
{
    /// <summary>
    /// Запускает обучение модели и связанные ML-процессы.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат выполнения обучения.</returns>
    Task<RunTrainingResponse> RunAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Генерирует прогнозы для клиентов.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция результатов прогнозирования.</returns>
    Task<IReadOnlyCollection<ForecastResultResponse>> GenerateForecastsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Генерирует рекомендации для клиентов.
    /// </summary>
    /// <param name="topPerClient">Количество рекомендаций на одного клиента.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция результатов рекомендаций.</returns>
    Task<IReadOnlyCollection<RecommendationResultResponse>> GenerateRecommendationsAsync(int topPerClient, CancellationToken cancellationToken);
}
