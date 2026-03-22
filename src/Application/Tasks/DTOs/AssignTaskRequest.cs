namespace Crm.Application.Tasks.DTOs;

/// <summary>
/// Запрос на назначение исполнителя задачи.
/// </summary>
public sealed class AssignTaskRequest
{
    /// <summary>
    /// Идентификатор пользователя-исполнителя.
    /// </summary>
    public Guid? AssignedToUserId { get; init; }
}