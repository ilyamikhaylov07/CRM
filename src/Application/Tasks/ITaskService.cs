using Crm.Application.Tasks.DTOs;

namespace Crm.Application.Tasks;

/// <summary>
/// Сервис управления задачами.
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Создает новую задачу.
    /// </summary>
    Task<Guid> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает задачу по идентификатору.
    /// </summary>
    Task<TaskResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает список задач с учетом фильтров.
    /// </summary>
    Task<IReadOnlyCollection<TaskListItemResponse>> GetAllAsync(
        GetTasksRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет задачу.
    /// </summary>
    Task UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Изменяет статус задачи.
    /// </summary>
    Task ChangeStatusAsync(Guid id, ChangeTaskStatusRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Назначает исполнителя задачи.
    /// </summary>
    Task AssignAsync(Guid id, AssignTaskRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет задачу.
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}