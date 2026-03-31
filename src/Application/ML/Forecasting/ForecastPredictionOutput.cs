namespace Crm.Application.ML.Forecasting;

/// <summary>
/// Результат предсказания модели прогнозирования.
/// </summary>
public sealed class ForecastPredictionOutput
{
    /// <summary>
    /// Предсказанное значение целевой переменной.
    /// </summary>
    public float Score { get; set; }
}