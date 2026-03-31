using Microsoft.ML;
using Microsoft.ML.Trainers;
using System.Text.Json;

namespace Crm.Application.ML.Recommendations;

/// <summary>
/// Сервис обучения, загрузки и использования модели рекомендаций.
/// </summary>
public sealed class RecommendationModelService(
    IRecommendationTrainingDataService trainingDataService,
    ILogger<RecommendationModelService> logger) : IRecommendationModelService
{
    private const int NumberOfIterations = 40;
    private const int ApproximationRank = 32;
    private readonly MLContext _mlContext = new(seed: 42);
    private readonly SemaphoreSlim _sync = new(1, 1);
    private PredictionEngine<RecommendationPredictionInput, RecommendationPredictionOutput>? _predictionEngine;
    private RecommendationModelInfo? _modelInfo;
    private Dictionary<Guid, uint> _clientKeys = [];
    private Dictionary<Guid, uint> _productKeys = [];

    /// <summary>
    /// Обучает модель рекомендаций, если обучающих данных достаточно.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Информация о модели или <see langword="null"/>, если данных недостаточно.</returns>
    public async Task<RecommendationModelInfo?> TrainIfPossibleAsync(CancellationToken cancellationToken)
    {
        await _sync.WaitAsync(cancellationToken);

        try
        {
            var statistics = await trainingDataService.GetStatisticsAsync(cancellationToken);
            if (!statistics.HasEnoughDataForHybrid)
            {
                logger.LogInformation(
                    "Обучение рекомендательной модели пропущено из-за недостатка данных. TrainingRowCount: {TrainingRowCount}, UniqueClientCount: {UniqueClientCount}, UniqueProductCount: {UniqueProductCount}.",
                    statistics.TrainingRowCount,
                    statistics.UniqueClientCount,
                    statistics.UniqueProductCount);

                return null;
            }

            var dataSet = await trainingDataService.BuildTrainingSetAsync(cancellationToken);
            var data = _mlContext.Data.LoadFromEnumerable(dataSet.Rows);
            var pipeline = _mlContext.Transforms.Conversion.ConvertType(
                    outputColumnName: nameof(RecommendationTrainingRow.ClientKey),
                    inputColumnName: nameof(RecommendationTrainingRow.ClientKey),
                    outputKind: Microsoft.ML.Data.DataKind.UInt32)
                .Append(_mlContext.Transforms.Conversion.ConvertType(
                    outputColumnName: nameof(RecommendationTrainingRow.ProductKey),
                    inputColumnName: nameof(RecommendationTrainingRow.ProductKey),
                    outputKind: Microsoft.ML.Data.DataKind.UInt32))
                .Append(_mlContext.Transforms.Conversion.MapValueToKey(
                    outputColumnName: nameof(RecommendationTrainingRow.ClientKey),
                    inputColumnName: nameof(RecommendationTrainingRow.ClientKey)))
                .Append(_mlContext.Transforms.Conversion.MapValueToKey(
                    outputColumnName: nameof(RecommendationTrainingRow.ProductKey),
                    inputColumnName: nameof(RecommendationTrainingRow.ProductKey)));
            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = nameof(RecommendationTrainingRow.ClientKey),
                MatrixRowIndexColumnName = nameof(RecommendationTrainingRow.ProductKey),
                LabelColumnName = nameof(RecommendationTrainingRow.Label),
                NumberOfIterations = NumberOfIterations,
                ApproximationRank = ApproximationRank
            };

            var model = pipeline
                .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(options))
                .Fit(data);
            var modelVersion = $"recommendation-{DateTime.UtcNow:yyyyMMddHHmmss}";
            var modelDirectory = Path.Combine(AppContext.BaseDirectory, "ml-models");
            Directory.CreateDirectory(modelDirectory);
            var modelPath = Path.Combine(modelDirectory, $"{modelVersion}.zip");
            var mappingPath = Path.Combine(modelDirectory, $"{modelVersion}.map.json");

            await using (var fileStream = File.Create(modelPath))
            {
                _mlContext.Model.Save(model, data.Schema, fileStream);
            }

            var mappings = new RecommendationKeyMappings
            {
                ClientKeys = dataSet.ClientKeys,
                ProductKeys = dataSet.ProductKeys
            };

            await File.WriteAllTextAsync(
                mappingPath,
                JsonSerializer.Serialize(mappings),
                cancellationToken);

            _predictionEngine = _mlContext.Model.CreatePredictionEngine<RecommendationPredictionInput, RecommendationPredictionOutput>(model);
            _clientKeys = dataSet.ClientKeys.ToDictionary();
            _productKeys = dataSet.ProductKeys.ToDictionary();
            _modelInfo = new RecommendationModelInfo
            {
                ModelVersion = modelVersion,
                ModelPath = modelPath,
                TrainingRowCount = dataSet.Statistics.TrainingRowCount,
                UniqueClientCount = dataSet.Statistics.UniqueClientCount,
                UniqueProductCount = dataSet.Statistics.UniqueProductCount,
                TrainedAtUtc = DateTime.UtcNow,
                IsUsableForHybrid = true
            };

            logger.LogInformation(
                "Обучена рекомендательная модель. ModelVersion: {ModelVersion}, TrainingRowCount: {TrainingRowCount}, UniqueClientCount: {UniqueClientCount}, UniqueProductCount: {UniqueProductCount}, NumberOfIterations: {NumberOfIterations}, ApproximationRank: {ApproximationRank}.",
                _modelInfo.ModelVersion,
                _modelInfo.TrainingRowCount,
                _modelInfo.UniqueClientCount,
                _modelInfo.UniqueProductCount,
                NumberOfIterations,
                ApproximationRank);

            return _modelInfo;
        }
        finally
        {
            _sync.Release();
        }
    }

    /// <summary>
    /// Возвращает информацию о текущей рекомендательной модели.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Информация о модели или <see langword="null"/>, если модель отсутствует.</returns>
    public async Task<RecommendationModelInfo?> GetCurrentModelInfoAsync(CancellationToken cancellationToken)
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
                return null;
            }

            var latestModelPath = Directory.GetFiles(modelDirectory, "recommendation-*.zip")
                .OrderByDescending(x => x)
                .FirstOrDefault();
            if (latestModelPath is null)
            {
                return null;
            }

            var mappingPath = Path.Combine(
                Path.GetDirectoryName(latestModelPath)!,
                $"{Path.GetFileNameWithoutExtension(latestModelPath)}.map.json");
            if (!File.Exists(mappingPath))
            {
                return null;
            }

            using var stream = File.OpenRead(latestModelPath);
            var model = _mlContext.Model.Load(stream, out _);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<RecommendationPredictionInput, RecommendationPredictionOutput>(model);

            var mappings = JsonSerializer.Deserialize<RecommendationKeyMappings>(await File.ReadAllTextAsync(mappingPath, cancellationToken));
            if (mappings is null)
            {
                return null;
            }

            _clientKeys = mappings.ClientKeys.ToDictionary();
            _productKeys = mappings.ProductKeys.ToDictionary();

            var statistics = await trainingDataService.GetStatisticsAsync(cancellationToken);
            _modelInfo = new RecommendationModelInfo
            {
                ModelVersion = Path.GetFileNameWithoutExtension(latestModelPath),
                ModelPath = latestModelPath,
                TrainingRowCount = statistics.TrainingRowCount,
                UniqueClientCount = statistics.UniqueClientCount,
                UniqueProductCount = statistics.UniqueProductCount,
                TrainedAtUtc = File.GetCreationTimeUtc(latestModelPath),
                IsUsableForHybrid = statistics.HasEnoughDataForHybrid
            };

            return _modelInfo;
        }
        finally
        {
            _sync.Release();
        }
    }

    /// <summary>
    /// Выполняет предсказание affinity для пары клиент-товар.
    /// </summary>
    /// <param name="clientId">Идентификатор клиента.</param>
    /// <param name="productId">Идентификатор товара.</param>
    /// <returns>Предсказанный score или <see langword="null"/>, если модель недоступна.</returns>
    public float? Predict(Guid clientId, Guid productId)
    {
        if (_predictionEngine is null
            || !_clientKeys.TryGetValue(clientId, out var clientKey)
            || !_productKeys.TryGetValue(productId, out var productKey))
        {
            return null;
        }

        return _predictionEngine.Predict(new RecommendationPredictionInput
        {
            ClientKey = clientKey,
            ProductKey = productKey
        }).Score;
    }

    private sealed class RecommendationKeyMappings
    {
        public required IReadOnlyDictionary<Guid, uint> ClientKeys { get; init; }

        public required IReadOnlyDictionary<Guid, uint> ProductKeys { get; init; }
    }
}
