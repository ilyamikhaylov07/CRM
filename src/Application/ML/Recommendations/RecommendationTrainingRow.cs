namespace Crm.Application.ML.Recommendations;

/// <summary>
/// Строка обучающей выборки для модели рекомендаций.
/// </summary>
public sealed class RecommendationTrainingRow
{
    /// <summary>
    /// Числовой ключ клиента для матричной факторизации.
    /// </summary>
    public uint ClientKey { get; init; }

    /// <summary>
    /// Числовой ключ товара для матричной факторизации.
    /// </summary>
    public uint ProductKey { get; init; }

    /// <summary>
    /// Целевое значение взаимодействия клиента с товаром.
    /// </summary>
    public float Label { get; init; }
}
