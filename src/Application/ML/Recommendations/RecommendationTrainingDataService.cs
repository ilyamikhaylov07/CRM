using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Crm.Application.ML.Recommendations;

/// <summary>
/// Сервис подготовки обучающих данных для рекомендательной модели.
/// </summary>
public sealed class RecommendationTrainingDataService(CrmDbContext dbContext) : IRecommendationTrainingDataService
{
    /// <summary>
    /// Строит обучающий датасет на основе истории покупок клиентов.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Обучающий датасет вместе со служебными key-map.</returns>
    public async Task<RecommendationTrainingDataSet> BuildTrainingSetAsync(CancellationToken cancellationToken)
    {
        var interactions = await LoadInteractionsAsync(cancellationToken);
        var clientKeys = interactions
            .Select(x => x.ClientId)
            .Distinct()
            .OrderBy(x => x)
            .Select((id, index) => new KeyValuePair<Guid, uint>(id, (uint)index + 1))
            .ToDictionary(x => x.Key, x => x.Value);
        var productKeys = interactions
            .Select(x => x.ProductId)
            .Distinct()
            .OrderBy(x => x)
            .Select((id, index) => new KeyValuePair<Guid, uint>(id, (uint)index + 1))
            .ToDictionary(x => x.Key, x => x.Value);

        var rows = interactions
            .Select(interaction => new RecommendationTrainingRow
            {
                ClientKey = clientKeys[interaction.ClientId],
                ProductKey = productKeys[interaction.ProductId],
                Label = Math.Max(1F, (float)interaction.Quantity)
            })
            .ToList();

        return new RecommendationTrainingDataSet
        {
            Rows = rows,
            ClientKeys = clientKeys,
            ProductKeys = productKeys,
            Statistics = BuildStatistics(interactions)
        };
    }

    /// <summary>
    /// Возвращает статистику датасета для принятия решения о hybrid-режиме.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Сводная статистика обучающих данных.</returns>
    public async Task<RecommendationTrainingStatistics> GetStatisticsAsync(CancellationToken cancellationToken)
    {
        var interactions = await LoadInteractionsAsync(cancellationToken);
        return BuildStatistics(interactions);
    }

    /// <summary>
    /// Загружает агрегированные взаимодействия клиент-товар.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список агрегированных взаимодействий.</returns>
    private async Task<List<ClientProductInteraction>> LoadInteractionsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.DealItems
            .AsNoTracking()
            .Select(x => new
            {
                x.ProductId,
                x.Quantity,
                x.Deal.ClientId
            })
            .GroupBy(x => new { x.ClientId, x.ProductId })
            .Select(x => new ClientProductInteraction
            {
                ClientId = x.Key.ClientId,
                ProductId = x.Key.ProductId,
                Quantity = x.Sum(i => i.Quantity)
            })
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Строит агрегированную статистику по взаимодействиям клиент-товар.
    /// </summary>
    /// <param name="interactions">Список агрегированных взаимодействий.</param>
    /// <returns>Сводная статистика датасета.</returns>
    private static RecommendationTrainingStatistics BuildStatistics(IReadOnlyCollection<ClientProductInteraction> interactions)
    {
        var uniqueClientCount = interactions.Select(x => x.ClientId).Distinct().Count();
        var uniqueProductCount = interactions.Select(x => x.ProductId).Distinct().Count();
        var clientsWithMultiplePurchasesCount = interactions
            .GroupBy(x => x.ClientId)
            .Count(x => x.Count() > 1);
        var productsWithMultiplePurchasesCount = interactions
            .GroupBy(x => x.ProductId)
            .Count(x => x.Count() > 1);

        return new RecommendationTrainingStatistics
        {
            TrainingRowCount = interactions.Count,
            UniqueClientCount = uniqueClientCount,
            UniqueProductCount = uniqueProductCount,
            ClientsWithMultiplePurchasesCount = clientsWithMultiplePurchasesCount,
            ProductsWithMultiplePurchasesCount = productsWithMultiplePurchasesCount,
            HasEnoughDataForHybrid = interactions.Count >= 50
                && uniqueClientCount >= 10
                && uniqueProductCount >= 10
                && clientsWithMultiplePurchasesCount >= 10
                && productsWithMultiplePurchasesCount >= 10
        };
    }

    private sealed class ClientProductInteraction
    {
        public Guid ClientId { get; init; }

        public Guid ProductId { get; init; }

        public decimal Quantity { get; init; }
    }
}
