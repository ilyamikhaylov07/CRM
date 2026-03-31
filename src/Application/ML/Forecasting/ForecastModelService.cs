using Crm.Application.Common.Exceptions;
using Microsoft.ML;

namespace Crm.Application.ML.Forecasting;

/// <summary>
/// Сервис для обучения, загрузки и использования модели прогнозирования продаж.
/// </summary>
public sealed class ForecastModelService(
    IForecastTrainingDataService trainingDataService,
    ILogger<ForecastModelService> logger) : IForecastModelService
{
    private static readonly string[] FeatureColumns =
    {
        nameof(ForecastTrainingRow.Age),
        nameof(ForecastTrainingRow.PreviousPurchases),
        nameof(ForecastTrainingRow.HistoricDealCount),
        nameof(ForecastTrainingRow.AveragePurchaseAmount),
        nameof(ForecastTrainingRow.TotalPurchaseAmount),
        nameof(ForecastTrainingRow.LastPurchaseAmount),
        nameof(ForecastTrainingRow.DiscountUsageRate),
        nameof(ForecastTrainingRow.PromoUsageRate),
        nameof(ForecastTrainingRow.AverageReviewRating),
        nameof(ForecastTrainingRow.ActivityCountLast30Days),
        nameof(ForecastTrainingRow.ActivityCountLast90Days),
        nameof(ForecastTrainingRow.OpenTaskCount),
        nameof(ForecastTrainingRow.CompletedTaskCount),
        nameof(ForecastTrainingRow.DaysSinceLastPurchase),
        nameof(ForecastTrainingRow.DaysSinceLastActivity),
        nameof(ForecastTrainingRow.AvgItemsPerDeal),
        nameof(ForecastTrainingRow.AvgDistinctProductsPerDeal),
        nameof(ForecastTrainingRow.PreferredCategoryShare),
        nameof(ForecastTrainingRow.IsMale),
        nameof(ForecastTrainingRow.IsFemale),
        nameof(ForecastTrainingRow.UsesDiscounts),
        nameof(ForecastTrainingRow.UsesPromoCodes)
    };

    private readonly MLContext _mlContext = new(seed: 42);
    private readonly SemaphoreSlim _sync = new(1, 1);
    private PredictionEngine<ForecastPredictionInput, ForecastPredictionOutput>? _predictionEngine;
    private ForecastModelInfo? _modelInfo;

    /// <summary>
    /// Обучает модель прогнозирования на исторических данных, сохраняет её на диск и возвращает информацию о модели.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Информация об обученной модели.</returns>
    public async Task<ForecastModelInfo> TrainAsync(CancellationToken cancellationToken)
    {
        await _sync.WaitAsync(cancellationToken);

        try
        {
            var rows = await trainingDataService.BuildTrainingSetAsync(cancellationToken);
            if (rows.Count < 5)
            {
                throw new ValidationException("Недостаточно исторических данных для обучения модели прогноза. Нужно минимум 5 обучающих строк.");
            }

            var data = _mlContext.Data.LoadFromEnumerable(rows);
            var split = _mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

            var pipeline = _mlContext.Transforms.Concatenate("Features", FeatureColumns)
                .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(_mlContext.Regression.Trainers.Sdca(
                    labelColumnName: nameof(ForecastTrainingRow.Label),
                    featureColumnName: "Features"));

            var model = pipeline.Fit(split.TrainSet);
            var predictions = model.Transform(split.TestSet);
            var metrics = _mlContext.Regression.Evaluate(predictions, labelColumnName: nameof(ForecastTrainingRow.Label));

            var modelVersion = $"forecast-{DateTime.UtcNow:yyyyMMddHHmmss}";
            var modelDirectory = Path.Combine(AppContext.BaseDirectory, "ml-models");
            Directory.CreateDirectory(modelDirectory);
            var modelPath = Path.Combine(modelDirectory, $"{modelVersion}.zip");

            await using (var fileStream = File.Create(modelPath))
            {
                _mlContext.Model.Save(model, data.Schema, fileStream);
            }

            _predictionEngine = _mlContext.Model.CreatePredictionEngine<ForecastPredictionInput, ForecastPredictionOutput>(model);
            _modelInfo = new ForecastModelInfo
            {
                ModelVersion = modelVersion,
                ModelPath = modelPath,
                TrainingRowCount = rows.Count,
                MeanAbsoluteError = metrics.MeanAbsoluteError,
                RootMeanSquaredError = metrics.RootMeanSquaredError,
                RSquared = metrics.RSquared,
                TrainedAtUtc = DateTime.UtcNow
            };

            logger.LogInformation(
                "Обучена модель прогноза. ModelVersion: {ModelVersion}, TrainingRowCount: {TrainingRowCount}, MAE: {MAE}, RMSE: {RMSE}, RSquared: {RSquared}.",
                _modelInfo.ModelVersion,
                _modelInfo.TrainingRowCount,
                _modelInfo.MeanAbsoluteError,
                _modelInfo.RootMeanSquaredError,
                _modelInfo.RSquared);

            return _modelInfo;
        }
        finally
        {
            _sync.Release();
        }
    }

    /// <summary>
    /// Возвращает информацию о текущей модели прогнозирования, загружая последнюю сохранённую модель при необходимости.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Информация о текущей модели.</returns>
    public async Task<ForecastModelInfo> GetCurrentModelInfoAsync(CancellationToken cancellationToken)
    {
        await _sync.WaitAsync(cancellationToken);

        try
        {
            if (_modelInfo is not null && _predictionEngine is not null)
            {
                return _modelInfo;
            }

            var modelDirectory = Path.Combine(AppContext.BaseDirectory, "ml-models");
            if (!Directory.Exists(modelDirectory))
            {
                throw new NotFoundException("Модель прогноза ещё не обучена.");
            }

            var latestModelPath = Directory.GetFiles(modelDirectory, "forecast-*.zip")
                .OrderByDescending(x => x)
                .FirstOrDefault() ?? throw new NotFoundException("Модель прогноза ещё не обучена.");

            using var stream = File.OpenRead(latestModelPath);
            var model = _mlContext.Model.Load(stream, out _);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<ForecastPredictionInput, ForecastPredictionOutput>(model);

            _modelInfo ??= new ForecastModelInfo
            {
                ModelVersion = Path.GetFileNameWithoutExtension(latestModelPath),
                ModelPath = latestModelPath,
                TrainingRowCount = 0,
                MeanAbsoluteError = 0,
                RootMeanSquaredError = 0,
                RSquared = 0,
                TrainedAtUtc = File.GetCreationTimeUtc(latestModelPath)
            };

            return _modelInfo;
        }
        finally
        {
            _sync.Release();
        }
    }

    /// <summary>
    /// Выполняет прогноз суммы продаж по входным признакам клиента.
    /// </summary>
    /// <param name="input">Входные признаки для модели прогнозирования.</param>
    /// <returns>Прогнозируемая сумма продаж.</returns>
    public decimal Predict(ForecastPredictionInput input)
    {
        if (_predictionEngine is null)
        {
            throw new ValidationException("Модель прогноза не загружена. Сначала выполните обучение.");
        }

        var prediction = _predictionEngine.Predict(input);
        return Math.Round(Math.Max(0M, (decimal)prediction.Score), 2, MidpointRounding.AwayFromZero);
    }
}
