using Crm.Domain.Enums;

namespace Crm.Application.Recommendations.DTOs;

/// <summary>
/// Запрос на изменение статуса рекомендации.
/// </summary>
public sealed class ChangeRecommendationStatusRequest
{
    /// <summary>
    /// Новый статус рекомендации.
    /// </summary>
    public RecommendationStatus Status { get; init; }
}
