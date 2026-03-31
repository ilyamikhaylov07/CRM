namespace Crm.Application.ML.Recommendations;

/// <summary>
/// Сервис обучения, загрузки и использования модели рекомендаций.
/// </summary>
public interface IRecommendationModelService
{
    /// <summary>
    /// Обучает модель, если данных достаточно для её использования.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Информация о модели или <see langword="null"/>, если данных недостаточно.</returns>
    Task<RecommendationModelInfo?> TrainIfPossibleAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает информацию о текущей модели рекомендаций.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Информация о модели или <see langword="null"/>, если модель отсутствует.</returns>
    Task<RecommendationModelInfo?> GetCurrentModelInfoAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Выполняет предсказание affinity для пары клиент-товар.
    /// </summary>
    /// <param name="clientId">Идентификатор клиента.</param>
    /// <param name="productId">Идентификатор товара.</param>
    /// <returns>Предсказанный score или <see langword="null"/>, если модель недоступна.</returns>
    float? Predict(Guid clientId, Guid productId);
}
