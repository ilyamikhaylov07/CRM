namespace Crm.Application.ML.Forecasting;

/// <summary>
/// Снимок признаков клиента, используемых для прогнозирования.
/// </summary>
public sealed class ClientForecastFeatureSnapshot
{
    /// <summary>
    /// Идентификатор клиента.
    /// </summary>
    public Guid ClientId { get; init; }

    /// <summary>
    /// Входные признаки для модели прогнозирования.
    /// </summary>
    public ForecastPredictionInput Input { get; init; } = new();

    /// <summary>
    /// Средняя сумма покупок клиента.
    /// </summary>
    public decimal AveragePurchaseAmount { get; init; }

    /// <summary>
    /// Общее количество сделок клиента за всё время.
    /// </summary>
    public int HistoricDealCount { get; init; }
}