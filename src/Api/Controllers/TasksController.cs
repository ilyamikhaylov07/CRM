using Auth.Core.Authorization;
using Crm.Application.Tasks;
using Crm.Application.Tasks.DTOs;
using Crm.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

/// <summary>
/// Контроллер управления задачами.
/// </summary>
/// <remarks>
/// Предоставляет операции создания, получения, обновления, удаления,
/// изменения статуса и назначения исполнителя задач.
/// </remarks>
[ApiController]
[Route("crm/tasks")]
[AuthorizeCrm(GlobalRoles = [SystemRoles.Admin, SystemRoles.Manager])]
public sealed class TasksController(ITaskService taskService) : ControllerBase
{
    /// <summary>
    /// Получает список задач с учетом фильтров.
    /// </summary>
    /// <param name="request">Параметры фильтрации списка задач.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция задач.</returns>
    /// <response code="200">Список задач успешно получен.</response>
    /// <response code="400">Переданы некорректные параметры фильтрации.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<TaskListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyCollection<TaskListItemResponse>>> GetAll(
        [FromQuery] GetTasksRequest request,
        CancellationToken cancellationToken)
    {
        var tasks = await taskService.GetAllAsync(request, cancellationToken);
        return Ok(tasks);
    }

    /// <summary>
    /// Получает задачу по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор задачи.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные задачи.</returns>
    /// <response code="200">Задача найдена.</response>
    /// <response code="404">Задача не найдена.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TaskResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var task = await taskService.GetByIdAsync(id, cancellationToken);
        return Ok(task);
    }

    /// <summary>
    /// Создает новую задачу.
    /// </summary>
    /// <param name="request">Данные задачи.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Идентификатор созданной задачи.</returns>
    /// <response code="201">Задача успешно создана.</response>
    /// <response code="400">Переданы некорректные данные.</response>
    /// <response code="404">Клиент, сделка или пользователь не найдены.</response>
    /// <response code="409">Нарушены бизнес-ограничения связи клиента и сделки.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var taskId = await taskService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = taskId },
            new
            {
                TaskId = taskId,
                Message = "Задача успешно создана."
            });
    }

    /// <summary>
    /// Обновляет задачу.
    /// </summary>
    /// <param name="id">Идентификатор задачи.</param>
    /// <param name="request">Новые данные задачи.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешном обновлении.</returns>
    /// <response code="204">Задача успешно обновлена.</response>
    /// <response code="400">Переданы некорректные данные.</response>
    /// <response code="404">Задача, клиент, сделка или пользователь не найдены.</response>
    /// <response code="409">Нарушены бизнес-ограничения связи клиента и сделки.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        await taskService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Изменяет статус задачи.
    /// </summary>
    /// <param name="id">Идентификатор задачи.</param>
    /// <param name="request">Новый статус задачи.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешном изменении статуса.</returns>
    /// <response code="204">Статус задачи успешно изменен.</response>
    /// <response code="404">Задача не найдена.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ChangeStatus(
        Guid id,
        [FromBody] ChangeTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        await taskService.ChangeStatusAsync(id, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Назначает исполнителя задачи.
    /// </summary>
    /// <param name="id">Идентификатор задачи.</param>
    /// <param name="request">Данные назначения исполнителя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешном назначении.</returns>
    /// <response code="204">Исполнитель задачи успешно изменен.</response>
    /// <response code="404">Задача или пользователь не найдены.</response>
    /// <response code="400">Переданы некорректные данные.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpPatch("{id:guid}/assign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Assign(
        Guid id,
        [FromBody] AssignTaskRequest request,
        CancellationToken cancellationToken)
    {
        await taskService.AssignAsync(id, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Удаляет задачу.
    /// </summary>
    /// <param name="id">Идентификатор задачи.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешном удалении.</returns>
    /// <response code="204">Задача успешно удалена.</response>
    /// <response code="404">Задача не найдена.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await taskService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}