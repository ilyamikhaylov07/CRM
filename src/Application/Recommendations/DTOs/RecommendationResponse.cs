using Crm.Domain.Enums;

namespace Crm.Application.Recommendations.DTOs;

/// <summary>
/// Полная информация о рекомендации.
/// </summary>
public sealed class RecommendationResponse
{
    /// <summary>
    /// Идентификатор рекомендации.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Идентификатор клиента.
    /// </summary>
    public Guid ClientId { get; init; }

    /// <summary>
    /// ФИО клиента.
    /// </summary>
    public string ClientFullName { get; init; } = string.Empty;

    /// <summary>
    /// Идентификатор товара.
    /// </summary>
    public Guid ProductId { get; init; }

    /// <summary>
    /// Название товара.
    /// </summary>
    public string ProductName { get; init; } = string.Empty;

    /// <summary>
    /// Категория товара.
    /// </summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Оценка релевантности.
    /// </summary>
    public decimal Score { get; init; }

    /// <summary>
    /// Причина формирования рекомендации.
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// Статус рекомендации.
    /// </summary>
    public RecommendationStatus Status { get; init; }

    /// <summary>
    /// Дата формирования рекомендации в UTC.
    /// </summary>
    public DateTime RecommendationDateUtc { get; init; }

    /// <summary>
    /// Дата создания записи в UTC.
    /// </summary>
    public DateTime CreatedAtUtc { get; init; }

    /// <summary>
    /// Дата обновления записи в UTC.
    /// </summary>
    public DateTime? UpdatedAtUtc { get; init; }
}
