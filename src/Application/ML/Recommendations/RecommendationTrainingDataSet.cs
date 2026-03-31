namespace Crm.Application.ML.Recommendations;

/// <summary>
/// Обучающий датасет рекомендательной модели вместе с key-map для клиентов и товаров.
/// </summary>
public sealed class RecommendationTrainingDataSet
{
    /// <summary>
    /// Строки обучающего датасета.
    /// </summary>
    public required IReadOnlyCollection<RecommendationTrainingRow> Rows { get; init; }

    /// <summary>
    /// Отображение идентификатора клиента в числовой ключ модели.
    /// </summary>
    public required IReadOnlyDictionary<Guid, uint> ClientKeys { get; init; }

    /// <summary>
    /// Отображение идентификатора товара в числовой ключ модели.
    /// </summary>
    public required IReadOnlyDictionary<Guid, uint> ProductKeys { get; init; }

    /// <summary>
    /// Статистика обучающих данных.
    /// </summary>
    public required RecommendationTrainingStatistics Statistics { get; init; }
}
