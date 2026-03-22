using Crm.Domain.Enums;

namespace Crm.Application.Activities.DTOs;

/// <summary>
/// Запрос на создание активности.
/// </summary>
public sealed class CreateActivityRequest
{
    /// <summary>
    /// Тип активности.
    /// </summary>
    public ActivityType Type { get; init; }

    /// <summary>
    /// Тема активности.
    /// </summary>
    public string? Subject { get; init; }

    /// <summary>
    /// Описание активности.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Дата активности в UTC.
    /// </summary>
    public DateTime ActivityDateUtc { get; init; }

    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Идентификатор клиента.
    /// </summary>
    public Guid ClientId { get; init; }

    /// <summary>
    /// Идентификатор сделки.
    /// </summary>
    public Guid? DealId { get; init; }
}