using TaskStatus = Crm.Domain.Enums.TaskStatus;

namespace Crm.Application.Tasks.DTOs;

/// <summary>
/// Параметры запроса списка задач.
/// </summary>
public sealed class GetTasksRequest
{
    /// <summary>
    /// Фильтр по клиенту.
    /// </summary>
    public Guid? ClientId { get; init; }

    /// <summary>
    /// Фильтр по сделке.
    /// </summary>
    public Guid? DealId { get; init; }

    /// <summary>
    /// Фильтр по исполнителю.
    /// </summary>
    public Guid? AssignedToUserId { get; init; }

    /// <summary>
    /// Фильтр по статусу.
    /// </summary>
    public TaskStatus? Status { get; init; }

    /// <summary>
    /// Поиск по заголовку или описанию.
    /// </summary>
    public string? Search { get; init; }

    /// <summary>
    /// Нижняя граница срока выполнения.
    /// </summary>
    public DateTime? DueDateFromUtc { get; init; }

    /// <summary>
    /// Верхняя граница срока выполнения.
    /// </summary>
    public DateTime? DueDateToUtc { get; init; }
}