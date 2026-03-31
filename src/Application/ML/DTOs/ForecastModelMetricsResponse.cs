namespace Crm.Application.ML.DTOs;

/// <summary>
/// Метрики обученной модели прогнозирования продаж.
/// </summary>
public sealed class ForecastModelMetricsResponse
{
    /// <summary>
    /// Версия модели.
    /// </summary>
    public required string ModelVersion { get; init; }

    /// <summary>
    /// Количество строк, использованных при обучении.
    /// </summary>
    public int TrainingRowCount { get; init; }

    /// <summary>
    /// Средняя абсолютная ошибка (MAE).
    /// </summary>
    public double MeanAbsoluteError { get; init; }

    /// <summary>
    /// Корень из средней квадратичной ошибки (RMSE).
    /// </summary>
    public double RootMeanSquaredError { get; init; }

    /// <summary>
    /// Коэффициент детерминации.
    /// </summary>
    public double RSquared { get; init; }
}
