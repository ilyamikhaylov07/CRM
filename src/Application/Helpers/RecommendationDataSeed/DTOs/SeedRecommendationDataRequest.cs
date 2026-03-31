namespace Crm.Application.Helpers.RecommendationDataSeed.DTOs;

/// <summary>
/// Запрос на генерацию тестовой истории покупок для рекомендательной модели.
/// </summary>
public sealed class SeedRecommendationDataRequest
{
    /// <summary>
    /// Количество новых сделок на одного клиента.
    /// </summary>
    public int DealsPerClient { get; init; } = 2;

    /// <summary>
    /// Минимальное количество товаров в одной сделке.
    /// </summary>
    public int MinItemsPerDeal { get; init; } = 2;

    /// <summary>
    /// Максимальное количество товаров в одной сделке.
    /// </summary>
    public int MaxItemsPerDeal { get; init; } = 4;

    /// <summary>
    /// Доля клиентов, для которых будет сгенерирована история, от 0 до 1.
    /// </summary>
    public decimal ClientCoverageRatio { get; init; } = 1.0M;

    /// <summary>
    /// Признак очистки существующих helper-сделок перед повторным запуском.
    /// </summary>
    public bool ReplacePreviousSeedData { get; init; } = false;
}
