namespace Crm.Application.ML.Forecasting;

/// <summary>
/// Определяет контракт сервиса для подготовки обучающих и прогнозных данных.
/// </summary>
public interface IForecastTrainingDataService
{
    /// <summary>
    /// Формирует обучающую выборку для модели прогнозирования.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция строк обучающей выборки.</returns>
    Task<IReadOnlyCollection<ForecastTrainingRow>> BuildTrainingSetAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Формирует набор признаков для выполнения прогноза по клиентам.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция снимков признаков клиентов.</returns>
    Task<IReadOnlyCollection<ClientForecastFeatureSnapshot>> BuildPredictionSetAsync(CancellationToken cancellationToken);
}
