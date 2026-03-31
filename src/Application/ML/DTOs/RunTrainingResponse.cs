namespace Crm.Application.ML.DTOs;

/// <summary>
/// Результат запуска обучения модели и генерации прогнозов и рекомендаций.
/// </summary>
public sealed class RunTrainingResponse
{
    /// <summary>
    /// Метрики обученной модели прогнозирования.
    /// </summary>
    public required ForecastModelMetricsResponse ForecastMetrics { get; init; }

    /// <summary>
    /// Количество сгенерированных прогнозов.
    /// </summary>
    public int ForecastsGenerated { get; init; }

    /// <summary>
    /// Количество сгенерированных рекомендаций.
    /// </summary>
    public int RecommendationsGenerated { get; init; }
}