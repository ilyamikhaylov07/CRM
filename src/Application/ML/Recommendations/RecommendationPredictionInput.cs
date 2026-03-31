namespace Crm.Application.ML.Recommendations;

/// <summary>
/// Входные данные для предсказания affinity между клиентом и товаром.
/// </summary>
public sealed class RecommendationPredictionInput
{
    /// <summary>
    /// Числовой ключ клиента.
    /// </summary>
    public uint ClientKey { get; init; }

    /// <summary>
    /// Числовой ключ товара.
    /// </summary>
    public uint ProductKey { get; init; }
}
