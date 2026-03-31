namespace Crm.Application.Forecasts.DTOs;

/// <summary>
/// Запрос на получение списка прогнозов.
/// </summary>
public sealed class GetForecastsRequest
{
    /// <summary>
    /// Идентификатор клиента.
    /// </summary>
    public Guid? ClientId { get; init; }

    /// <summary>
    /// Версия модели прогноза.
    /// </summary>
    public string? ModelVersion { get; init; }

    /// <summary>
    /// Начальная дата формирования прогноза в UTC.
    /// </summary>
    public DateTime? FromDateUtc { get; init; }

    /// <summary>
    /// Конечная дата формирования прогноза в UTC.
    /// </summary>
    public DateTime? ToDateUtc { get; init; }

    /// <summary>
    /// Начало прогнозируемого периода в UTC.
    /// </summary>
    public DateTime? PeriodStartUtc { get; init; }

    /// <summary>
    /// Конец прогнозируемого периода в UTC.
    /// </summary>
    public DateTime? PeriodEndUtc { get; init; }
}
