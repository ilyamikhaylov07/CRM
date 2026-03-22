using TaskStatus = Crm.Domain.Enums.TaskStatus;

namespace Crm.Application.Tasks.DTOs;

/// <summary>
/// Краткая информация о задаче для списков.
/// </summary>
public sealed class TaskListItemResponse
{
    /// <summary>
    /// Идентификатор задачи.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Заголовок задачи.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Срок выполнения в UTC.
    /// </summary>
    public DateTime DueDateUtc { get; init; }

    /// <summary>
    /// Статус задачи.
    /// </summary>
    public TaskStatus Status { get; init; }

    /// <summary>
    /// Идентификатор исполнителя.
    /// </summary>
    public Guid? AssignedToUserId { get; init; }

    /// <summary>
    /// Полное имя исполнителя.
    /// </summary>
    public string? AssignedToUserFullName { get; init; }

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