namespace Crm.Application.ML.Forecasting;

/// <summary>
/// Определяет контракт сервиса для обучения, загрузки и использования модели прогнозирования продаж.
/// </summary>
public interface IForecastModelService
{
    /// <summary>
    /// Обучает модель прогнозирования и возвращает информацию о ней.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Информация об обученной модели.</returns>
    Task<ForecastModelInfo> TrainAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает информацию о текущей модели прогнозирования.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Информация о текущей модели.</returns>
    Task<ForecastModelInfo> GetCurrentModelInfoAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Выполняет прогноз по входным признакам клиента.
    /// </summary>
    /// <param name="input">Входные признаки для модели прогнозирования.</param>
    /// <returns>Прогнозируемая сумма.</returns>
    decimal Predict(ForecastPredictionInput input);
}
