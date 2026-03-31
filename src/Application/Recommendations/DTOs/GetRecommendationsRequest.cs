using Crm.Domain.Enums;

namespace Crm.Application.Recommendations.DTOs;

/// <summary>
/// Запрос на получение списка рекомендаций.
/// </summary>
public sealed class GetRecommendationsRequest
{
    /// <summary>
    /// Идентификатор клиента.
    /// </summary>
    public Guid? ClientId { get; init; }

    /// <summary>
    /// Идентификатор товара.
    /// </summary>
    public Guid? ProductId { get; init; }

    /// <summary>
    /// Статус рекомендации.
    /// </summary>
    public RecommendationStatus? Status { get; init; }

    /// <summary>
    /// Начальная дата формирования рекомендации в UTC.
    /// </summary>
    public DateTime? FromDateUtc { get; init; }

    /// <summary>
    /// Конечная дата формирования рекомендации в UTC.
    /// </summary>
    public DateTime? ToDateUtc { get; init; }
}
