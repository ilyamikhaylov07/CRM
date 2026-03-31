namespace Crm.Application.Helpers.RecommendationDataSeed.DTOs;

/// <summary>
/// Результат генерации тестовой истории покупок.
/// </summary>
public sealed class SeedRecommendationDataResponse
{
    /// <summary>
    /// Количество клиентов, затронутых генерацией.
    /// </summary>
    public int ClientsAffected { get; init; }

    /// <summary>
    /// Количество созданных сделок.
    /// </summary>
    public int DealsCreated { get; init; }

    /// <summary>
    /// Количество созданных позиций сделок.
    /// </summary>
    public int DealItemsCreated { get; init; }

    /// <summary>
    /// Число категорий, использованных при генерации.
    /// </summary>
    public int CategoriesUsed { get; init; }

    /// <summary>
    /// Техническое сообщение о выполненной генерации.
    /// </summary>
    public string Message { get; init; } = string.Empty;
}
