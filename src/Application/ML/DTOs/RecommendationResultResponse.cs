using Crm.Domain.Enums;

namespace Crm.Application.ML.DTOs;

/// <summary>
/// Результат рекомендации товара для клиента.
/// </summary>
public sealed class RecommendationResultResponse
{
    /// <summary>
    /// Идентификатор клиента.
    /// </summary>
    public Guid ClientId { get; init; }

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
    /// Оценка релевантности рекомендации.
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
    /// Дата формирования рекомендации (UTC).
    /// </summary>
    public DateTime RecommendationDateUtc { get; init; }
}