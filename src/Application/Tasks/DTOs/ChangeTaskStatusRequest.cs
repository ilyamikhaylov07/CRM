using TaskStatus = Crm.Domain.Enums.TaskStatus;

namespace Crm.Application.Tasks.DTOs;

/// <summary>
/// Запрос на изменение статуса задачи.
/// </summary>
public sealed class ChangeTaskStatusRequest
{
    /// <summary>
    /// Новый статус задачи.
    /// </summary>
    public TaskStatus Status { get; init; }
}