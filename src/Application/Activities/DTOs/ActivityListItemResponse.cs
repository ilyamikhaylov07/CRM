using Crm.Domain.Enums;

namespace Crm.Application.Activities.DTOs;

/// <summary>
/// Краткая информация об активности для списков.
/// </summary>
public sealed class ActivityListItemResponse
{
    /// <summary>
    /// Идентификатор активности.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Тип активности.
    /// </summary>
    public ActivityType Type { get; init; }

    /// <summary>
    /// Тема активности.
    /// </summary>
    public string? Subject { get; init; }

    /// <summary>
    /// Дата активности в UTC.
    /// </summary>
    public DateTime ActivityDateUtc { get; init; }

    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Полное имя пользователя.
    /// </summary>
    public string? UserFullName { get; init; }

    /// <summary>
    /// Идентификатор клиента.
    /// </summary>
    public Guid ClientId { get; init; }

    /// <summary>
    /// Имя клиента.
    /// </summary>
    public required string ClientName { get; init; }

    /// <summary>
    /// Идентификатор сделки.
    /// </summary>
    public Guid? DealId { get; init; }
}