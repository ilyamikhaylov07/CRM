namespace Crm.Application.ML.Recommendations;

/// <summary>
/// Сервис подготовки обучающих данных для рекомендательной модели.
/// </summary>
public interface IRecommendationTrainingDataService
{
    /// <summary>
    /// Строит обучающий датасет для модели рекомендаций.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Обучающий датасет вместе со служебными key-map.</returns>
    Task<RecommendationTrainingDataSet> BuildTrainingSetAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает статистику датасета, используемую для принятия решения о hybrid-режиме.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Сводная статистика обучающих данных.</returns>
    Task<RecommendationTrainingStatistics> GetStatisticsAsync(CancellationToken cancellationToken);
}
