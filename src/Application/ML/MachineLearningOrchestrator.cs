using Crm.Application.ML.DTOs;
using Crm.Application.ML.Forecasting;
using Crm.Application.ML.Recommendations;
using Crm.Domain.Entities;
using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Crm.Application.ML;

/// <summary>
/// Оркестратор процессов обучения модели, генерации прогнозов и рекомендаций.
/// </summary>
public sealed class MachineLearningOrchestrator(
    IForecastModelService forecastModelService,
    IForecastTrainingDataService trainingDataService,
    IRecommendationEngineService recommendationEngineService,
    CrmDbContext dbContext,
    ILogger<MachineLearningOrchestrator> logger) : IMachineLearningOrchestrator
{
    /// <summary>
    /// Запускает полный ML-сценарий: обучение модели, генерацию прогнозов и рекомендаций.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Сводный результат выполнения ML-процессов.</returns>
    public async Task<RunTrainingResponse> RunAsync(CancellationToken cancellationToken)
    {
        var metrics = await forecastModelService.TrainAsync(cancellationToken);
        var forecasts = await GenerateForecastsAsync(cancellationToken);
        var recommendations = await GenerateRecommendationsAsync(3, cancellationToken);

        return new RunTrainingResponse
        {
            ForecastMetrics = new ForecastModelMetricsResponse
            {
                ModelVersion = metrics.ModelVersion,
                TrainingRowCount = metrics.TrainingRowCount,
                MeanAbsoluteError = metrics.MeanAbsoluteError,
                RootMeanSquaredError = metrics.RootMeanSquaredError,
                RSquared = metrics.RSquared
            },
            ForecastsGenerated = forecasts.Count,
            RecommendationsGenerated = recommendations.Count
        };
    }

    /// <summary>
    /// Генерирует прогнозы продаж для клиентов и сохраняет их в базе данных.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция сгенерированных прогнозов.</returns>
    public async Task<IReadOnlyCollection<ForecastResultResponse>> GenerateForecastsAsync(CancellationToken cancellationToken)
    {
        var modelInfo = await forecastModelService.GetCurrentModelInfoAsync(cancellationToken);
        var predictionSet = await trainingDataService.BuildPredictionSetAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var periodStartUtc = now.Date;
        var periodEndUtc = now.Date.AddDays(30);

        var existingForecasts = await dbContext.SalesForecasts
            .Where(x => x.PeriodStartUtc == periodStartUtc && x.PeriodEndUtc == periodEndUtc)
            .ToListAsync(cancellationToken);

        if (existingForecasts.Count > 0)
        {
            dbContext.SalesForecasts.RemoveRange(existingForecasts);
        }

        var forecasts = new List<SalesForecast>();
        var responses = new List<ForecastResultResponse>();

        foreach (var snapshot in predictionSet)
        {
            var predictedAmount = forecastModelService.Predict(snapshot.Input);
            var confidence = CalculateConfidence(snapshot.HistoricDealCount);
            var notes = $"HistoricDealCount={snapshot.HistoricDealCount}; AvgPurchase={snapshot.AveragePurchaseAmount:F2}";

            forecasts.Add(new SalesForecast
            {
                ClientId = snapshot.ClientId,
                Client = null!,
                ForecastDateUtc = now,
                PeriodStartUtc = periodStartUtc,
                PeriodEndUtc = periodEndUtc,
                PredictedAmount = predictedAmount,
                ConfidenceScore = confidence,
                ModelVersion = modelInfo.ModelVersion,
                Notes = notes
            });

            responses.Add(new ForecastResultResponse
            {
                ClientId = snapshot.ClientId,
                ForecastDateUtc = now,
                PeriodStartUtc = periodStartUtc,
                PeriodEndUtc = periodEndUtc,
                PredictedAmount = predictedAmount,
                ConfidenceScore = confidence,
                ModelVersion = modelInfo.ModelVersion,
                Notes = notes
            });
        }

        if (forecasts.Count > 0)
        {
            await dbContext.SalesForecasts.AddRangeAsync(forecasts, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Сгенерированы прогнозы продаж. Count: {Count}, ModelVersion: {ModelVersion}.",
            forecasts.Count,
            modelInfo.ModelVersion);

        return responses;
    }

    /// <summary>
    /// Генерирует рекомендации товаров для клиентов.
    /// </summary>
    /// <param name="topPerClient">Количество рекомендаций на одного клиента.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция сгенерированных рекомендаций.</returns>
    public async Task<IReadOnlyCollection<RecommendationResultResponse>> GenerateRecommendationsAsync(int topPerClient, CancellationToken cancellationToken)
    {
        return await recommendationEngineService.GenerateAndStoreAsync(topPerClient, cancellationToken);
    }

    /// <summary>
    /// Вычисляет условную оценку уверенности прогноза на основе количества исторических сделок.
    /// </summary>
    /// <param name="historicDealCount">Количество исторических сделок клиента.</param>
    /// <returns>Оценка уверенности прогноза.</returns>
    private static decimal CalculateConfidence(int historicDealCount)
    {
        var confidence = Math.Min(0.95M, 0.35M + (historicDealCount * 0.05M));
        return Math.Round(confidence, 4, MidpointRounding.AwayFromZero);
    }
}
