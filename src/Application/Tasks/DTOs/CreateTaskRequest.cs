using TaskStatus = Crm.Domain.Enums.TaskStatus;

namespace Crm.Application.Tasks.DTOs;

/// <summary>
/// Запрос на создание задачи.
/// </summary>
public sealed class CreateTaskRequest
{
    /// <summary>
    /// Заголовок задачи.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Описание задачи.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Срок выполнения задачи в UTC.
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
    /// Идентификатор клиента.
    /// </summary>
    public Guid ClientId { get; init; }

    /// <summary>
    /// Идентификатор сделки.
    /// </summary>
    public Guid? DealId { get; init; }
}