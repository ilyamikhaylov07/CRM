namespace Crm.Application.ML.Recommendations;

/// <summary>
/// Информация об обученной модели рекомендаций.
/// </summary>
public sealed class RecommendationModelInfo
{
    /// <summary>
    /// Версия модели.
    /// </summary>
    public required string ModelVersion { get; init; }

    /// <summary>
    /// Путь к сохранённой модели.
    /// </summary>
    public required string ModelPath { get; init; }

    /// <summary>
    /// Количество строк, использованных при обучении.
    /// </summary>
    public int TrainingRowCount { get; init; }

    /// <summary>
    /// Количество уникальных клиентов в датасете.
    /// </summary>
    public int UniqueClientCount { get; init; }

    /// <summary>
    /// Количество уникальных товаров в датасете.
    /// </summary>
    public int UniqueProductCount { get; init; }

    /// <summary>
    /// Время обучения модели в UTC.
    /// </summary>
    public DateTime TrainedAtUtc { get; init; }

    /// <summary>
    /// Признак пригодности модели для hybrid-режима.
    /// </summary>
    public bool IsUsableForHybrid { get; init; }
}
