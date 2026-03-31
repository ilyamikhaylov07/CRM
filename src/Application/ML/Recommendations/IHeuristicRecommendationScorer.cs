using Crm.Domain.Entities;

namespace Crm.Application.ML.Recommendations;

/// <summary>
/// Сервис эвристического скоринга рекомендаций.
/// </summary>
public interface IHeuristicRecommendationScorer
{
    /// <summary>
    /// Считает эвристический score товара для конкретного клиента.
    /// </summary>
    /// <param name="client">Клиент, для которого строится рекомендация.</param>
    /// <param name="product">Кандидат в рекомендацию.</param>
    /// <param name="globalPopularity">Словарь глобальной популярности товаров.</param>
    /// <returns>Итоговый эвристический score.</returns>
    decimal Score(Client client, Product product, IReadOnlyDictionary<Guid, decimal> globalPopularity);
}
