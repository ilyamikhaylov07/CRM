using Crm.Domain.Entities;

namespace Crm.Application.ML.Recommendations;

/// <summary>
/// Сервис эвристического скоринга рекомендаций.
/// </summary>
public sealed class HeuristicRecommendationScorer : IHeuristicRecommendationScorer
{
    private readonly Dictionary<Guid, ClientHeuristicSnapshot> _clientCache = [];

    /// <summary>
    /// Считает эвристический score товара для конкретного клиента.
    /// </summary>
    /// <param name="client">Клиент, для которого строится рекомендация.</param>
    /// <param name="product">Кандидат в рекомендацию.</param>
    /// <param name="globalPopularity">Словарь глобальной популярности товаров.</param>
    /// <returns>Итоговый эвристический score.</returns>
    public decimal Score(Client client, Product product, IReadOnlyDictionary<Guid, decimal> globalPopularity)
    {
        if (!_clientCache.TryGetValue(client.Id, out var snapshot))
        {
            var purchasedItems = client.Deals
                .SelectMany(x => x.Items)
                .Where(x => x.Product is not null)
                .ToList();

            var categoryWeights = purchasedItems
                .GroupBy(x => x.Product.Category)
                .ToDictionary(x => x.Key, x => x.Sum(item => item.Quantity));

            snapshot = new ClientHeuristicSnapshot
            {
                CategoryWeights = categoryWeights,
                AverageClientPrice = purchasedItems.Count == 0
                    ? 0M
                    : purchasedItems.Average(x => x.UnitPrice),
                TotalCategoryQuantity = Math.Max(1M, categoryWeights.Values.Sum())
            };

            _clientCache[client.Id] = snapshot;
        }

        globalPopularity.TryGetValue(product.Id, out var popularity);
        var maxPopularity = globalPopularity.Count == 0 ? 1M : globalPopularity.Max(x => x.Value);
        snapshot.CategoryWeights.TryGetValue(product.Category, out var categoryQuantity);

        var categoryScore = snapshot.CategoryWeights.Count == 0
            ? 0.15M
            : categoryQuantity / snapshot.TotalCategoryQuantity;
        var priceScore = snapshot.AverageClientPrice <= 0M
            ? 0.15M
            : Math.Max(0M, 1M - (Math.Abs(product.BasePrice - snapshot.AverageClientPrice) / Math.Max(snapshot.AverageClientPrice, 1M)));
        var popularityScore = popularity / Math.Max(maxPopularity, 1M);

        return Math.Round((categoryScore * 0.5M) + (priceScore * 0.2M) + (popularityScore * 0.3M), 4, MidpointRounding.AwayFromZero);
    }

    private sealed class ClientHeuristicSnapshot
    {
        public required Dictionary<string, decimal> CategoryWeights { get; init; }

        public decimal AverageClientPrice { get; init; }

        public decimal TotalCategoryQuantity { get; init; }
    }
}
