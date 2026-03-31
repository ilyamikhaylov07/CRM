namespace Crm.Application.ML.Forecasting;

/// <summary>
/// Информация об обученной модели прогнозирования.
/// </summary>
public sealed class ForecastModelInfo
{
    /// <summary>
    /// Версия модели.
    /// </summary>
    public required string ModelVersion { get; init; }

    /// <summary>
    /// Путь к файлу модели.
    /// </summary>
    public required string ModelPath { get; init; }

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
    /// Коэффициент детерминации (R²).
    /// </summary>
    public double RSquared { get; init; }

    /// <summary>
    /// Дата и время обучения модели (UTC).
    /// </summary>
    public DateTime TrainedAtUtc { get; init; }
}