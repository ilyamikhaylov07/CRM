namespace Crm.Application.ML.Recommendations;

/// <summary>
/// Сводная статистика обучающего датасета рекомендаций.
/// </summary>
public sealed class RecommendationTrainingStatistics
{
    /// <summary>
    /// Общее число строк в датасете.
    /// </summary>
    public int TrainingRowCount { get; init; }

    /// <summary>
    /// Количество уникальных клиентов.
    /// </summary>
    public int UniqueClientCount { get; init; }

    /// <summary>
    /// Количество уникальных товаров.
    /// </summary>
    public int UniqueProductCount { get; init; }

    /// <summary>
    /// Количество клиентов, у которых больше одной покупки.
    /// </summary>
    public int ClientsWithMultiplePurchasesCount { get; init; }

    /// <summary>
    /// Количество товаров, которые покупались более одного раза.
    /// </summary>
    public int ProductsWithMultiplePurchasesCount { get; init; }

    /// <summary>
    /// Признак достаточности данных для hybrid-режима.
    /// </summary>
    public bool HasEnoughDataForHybrid { get; init; }
}
