namespace Crm.Application.ML.Recommendations;

/// <summary>
/// Результат предсказания affinity модели рекомендаций.
/// </summary>
public sealed class RecommendationPredictionOutput
{
    /// <summary>
    /// Предсказанный score взаимодействия клиента с товаром.
    /// </summary>
    public float Score { get; init; }
}
