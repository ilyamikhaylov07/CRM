using Auth.Core.Authorization;
using Crm.Application.Activities;
using Crm.Application.Activities.DTOs;
using Crm.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

/// <summary>
/// Контроллер управления активностями.
/// </summary>
/// <remarks>
/// Предоставляет операции создания, получения, обновления и удаления активностей.
/// </remarks>
[ApiController]
[Route("crm/activities")]
[AuthorizeCrm(GlobalRoles = [SystemRoles.Admin, SystemRoles.Manager, SystemRoles.HeadManager])]
public sealed class ActivitiesController(IActivityService activityService) : ControllerBase
{
    /// <summary>
    /// Получает список активностей с учетом фильтров.
    /// </summary>
    /// <param name="request">Параметры фильтрации списка активностей.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция активностей.</returns>
    /// <response code="200">Список активностей успешно получен.</response>
    /// <response code="400">Переданы некорректные параметры фильтрации.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ActivityListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyCollection<ActivityListItemResponse>>> GetAll(
        [FromQuery] GetActivitiesRequest request,
        CancellationToken cancellationToken)
    {
        var activities = await activityService.GetAllAsync(request, cancellationToken);
        return Ok(activities);
    }

    /// <summary>
    /// Получает активность по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор активности.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные активности.</returns>
    /// <response code="200">Активность найдена.</response>
    /// <response code="404">Активность не найдена.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ActivityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ActivityResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var activity = await activityService.GetByIdAsync(id, cancellationToken);
        return Ok(activity);
    }

    /// <summary>
    /// Создает новую активность.
    /// </summary>
    /// <param name="request">Данные активности.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Идентификатор созданной активности.</returns>
    /// <response code="201">Активность успешно создана.</response>
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
        [FromBody] CreateActivityRequest request,
        CancellationToken cancellationToken)
    {
        var activityId = await activityService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = activityId },
            new
            {
                ActivityId = activityId,
                Message = "Активность успешно создана."
            });
    }

    /// <summary>
    /// Обновляет активность.
    /// </summary>
    /// <param name="id">Идентификатор активности.</param>
    /// <param name="request">Новые данные активности.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешном обновлении.</returns>
    /// <response code="204">Активность успешно обновлена.</response>
    /// <response code="400">Переданы некорректные данные.</response>
    /// <response code="404">Активность, клиент, сделка или пользователь не найдены.</response>
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
        [FromBody] UpdateActivityRequest request,
        CancellationToken cancellationToken)
    {
        await activityService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Удаляет активность.
    /// </summary>
    /// <param name="id">Идентификатор активности.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешном удалении.</returns>
    /// <response code="204">Активность успешно удалена.</response>
    /// <response code="404">Активность не найдена.</response>
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
        await activityService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}