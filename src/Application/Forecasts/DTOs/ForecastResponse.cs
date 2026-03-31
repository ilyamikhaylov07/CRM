namespace Crm.Application.Forecasts.DTOs;

/// <summary>
/// Полная информация о прогнозе продаж.
/// </summary>
public sealed class ForecastResponse
{
    /// <summary>
    /// Идентификатор прогноза.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Идентификатор клиента.
    /// </summary>
    public Guid ClientId { get; init; }

    /// <summary>
    /// Имя клиента.
    /// </summary>
    public string ClientName { get; init; } = string.Empty;

    /// <summary>
    /// Дата формирования прогноза в UTC.
    /// </summary>
    public DateTime ForecastDateUtc { get; init; }

    /// <summary>
    /// Начало прогнозируемого периода в UTC.
    /// </summary>
    public DateTime PeriodStartUtc { get; init; }

    /// <summary>
    /// Конец прогнозируемого периода в UTC.
    /// </summary>
    public DateTime PeriodEndUtc { get; init; }

    /// <summary>
    /// Прогнозируемая сумма.
    /// </summary>
    public decimal PredictedAmount { get; init; }

    /// <summary>
    /// Оценка уверенности прогноза.
    /// </summary>
    public decimal? ConfidenceScore { get; init; }

    /// <summary>
    /// Версия модели.
    /// </summary>
    public string? ModelVersion { get; init; }

    /// <summary>
    /// Дополнительные примечания.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Дата создания записи в UTC.
    /// </summary>
    public DateTime CreatedAtUtc { get; init; }

    /// <summary>
    /// Дата обновления записи в UTC.
    /// </summary>
    public DateTime? UpdatedAtUtc { get; init; }
}
