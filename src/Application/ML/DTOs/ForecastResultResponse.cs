namespace Crm.Application.ML.DTOs;

/// <summary>
/// Результат прогноза продаж для клиента за указанный период.
/// </summary>
public sealed class ForecastResultResponse
{
    /// <summary>
    /// Идентификатор клиента.
    /// </summary>
    public Guid ClientId { get; init; }

    /// <summary>
    /// Дата формирования прогноза.
    /// </summary>
    public DateTime ForecastDateUtc { get; init; }

    /// <summary>
    /// Начало прогнозируемого периода.
    /// </summary>
    public DateTime PeriodStartUtc { get; init; }

    /// <summary>
    /// Конец прогнозируемого периода.
    /// </summary>
    public DateTime PeriodEndUtc { get; init; }

    /// <summary>
    /// Прогнозируемая сумма.
    /// </summary>
    public decimal PredictedAmount { get; init; }

    /// <summary>
    /// Оценка уверенности модели в прогнозе.
    /// </summary>
    public decimal? ConfidenceScore { get; init; }

    /// <summary>
    /// Версия использованной модели.
    /// </summary>
    public string? ModelVersion { get; init; }

    /// <summary>
    /// Дополнительные примечания.
    /// </summary>
    public string? Notes { get; init; }
}