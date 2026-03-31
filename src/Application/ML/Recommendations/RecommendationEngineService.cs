using Crm.Application.ML.DTOs;
using Crm.Domain.Entities;
using Crm.Domain.Enums;
using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Crm.Application.ML.Recommendations;

/// <summary>
/// Сервис генерации и сохранения рекомендаций в heuristic-only или hybrid-режиме.
/// </summary>
public sealed class RecommendationEngineService(
    IRecommendationModelService recommendationModelService,
    IHeuristicRecommendationScorer heuristicRecommendationScorer,
    CrmDbContext dbContext,
    ILogger<RecommendationEngineService> logger) : IRecommendationEngineService
{
    /// <summary>
    /// Генерирует и сохраняет рекомендации для клиентов.
    /// </summary>
    /// <param name="topPerClient">Количество рекомендаций на одного клиента.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список сохранённых рекомендаций.</returns>
    public async Task<IReadOnlyCollection<RecommendationResultResponse>> GenerateAndStoreAsync(int topPerClient, CancellationToken cancellationToken)
    {
        if (topPerClient <= 0)
        {
            topPerClient = 3;
        }

        var clients = await dbContext.Clients
            .Include(x => x.Deals)
            .ThenInclude(x => x.Items)
            .ThenInclude(x => x.Product)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        var products = await dbContext.Products
            .Where(x => x.IsActive)
            .ToListAsync(cancellationToken);

        var globalPopularity = await dbContext.DealItems
            .AsNoTracking()
            .GroupBy(x => x.ProductId)
            .Select(x => new { x.Key, Quantity = x.Sum(item => item.Quantity) })
            .ToDictionaryAsync(x => x.Key, x => x.Quantity, cancellationToken);

        var existingRecommendations = await dbContext.Recommendations
            .Where(x => x.Status == RecommendationStatus.New)
            .ExecuteDeleteAsync(cancellationToken);

        var modelInfo = await recommendationModelService.TrainIfPossibleAsync(cancellationToken)
            ?? await recommendationModelService.GetCurrentModelInfoAsync(cancellationToken);
        var useHybrid = modelInfo?.IsUsableForHybrid == true;

        var responses = new List<RecommendationResultResponse>();
        var entities = new List<Recommendation>();

        foreach (var client in clients)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var purchasedProductIds = client.Deals
                .SelectMany(x => x.Items)
                .Select(x => x.ProductId)
                .ToHashSet();

            var recommendations = products
                .Where(x => !purchasedProductIds.Contains(x.Id))
                .Select(product =>
                {
                    var heuristicScore = heuristicRecommendationScorer.Score(client, product, globalPopularity);
                    var mlScore = useHybrid
                        ? recommendationModelService.Predict(client.Id, product.Id)
                        : null;
                    var normalizedMlScore = NormalizeMlScore(mlScore);
                    var finalScore = mlScore.HasValue
                        ? Math.Round((heuristicScore * 0.8M) + (normalizedMlScore * 0.2M), 4, MidpointRounding.AwayFromZero)
                        : heuristicScore;
                    var reason = mlScore.HasValue
                        ? $"mode=hybrid; heuristic={heuristicScore:F4}; ml={normalizedMlScore:F4}; category={product.Category}"
                        : $"mode=heuristic-only; heuristic={heuristicScore:F4}; category={product.Category}";

                    return new
                    {
                        Product = product,
                        Score = finalScore,
                        Reason = reason
                    };
                })
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Product.Name)
                .Take(topPerClient)
                .ToList();

            foreach (var recommendation in recommendations)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var now = DateTime.UtcNow;
                entities.Add(new Recommendation
                {
                    ClientId = client.Id,
                    Client = client,
                    ProductId = recommendation.Product.Id,
                    Product = null!,
                    RecommendationDateUtc = now,
                    Score = recommendation.Score,
                    Reason = recommendation.Reason,
                    Status = RecommendationStatus.New
                });

                responses.Add(new RecommendationResultResponse
                {
                    ClientId = client.Id,
                    ProductId = recommendation.Product.Id,
                    ProductName = recommendation.Product.Name,
                    Category = recommendation.Product.Category,
                    Score = recommendation.Score,
                    Reason = recommendation.Reason,
                    Status = RecommendationStatus.New,
                    RecommendationDateUtc = now
                });
            }
        }

        if (entities.Count > 0)
        {
            await dbContext.Recommendations.AddRangeAsync(entities, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Сгенерированы рекомендации. Count: {Count}, TopPerClient: {TopPerClient}, Mode: {Mode}.",
            entities.Count,
            topPerClient,
            useHybrid ? "hybrid" : "heuristic-only");

        return responses;
    }

    /// <summary>
    /// Нормализует raw score матричной факторизации в диапазон от 0 до 1.
    /// </summary>
    /// <param name="mlScore">Сырой score модели.</param>
    /// <returns>Нормализованный score.</returns>
    private static decimal NormalizeMlScore(float? mlScore)
    {
        if (!mlScore.HasValue)
        {
            return 0M;
        }

        var normalized = 1D / (1D + Math.Exp(-mlScore.Value));
        return Math.Round((decimal)normalized, 4, MidpointRounding.AwayFromZero);
    }
}
