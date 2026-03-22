using Auth.Core.Authorization;
using Crm.Application.Deals;
using Crm.Application.Deals.DTOs;
using Crm.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

/// <summary>
/// Контроллер управления сделками.
/// </summary>
/// <remarks>
/// Предоставляет операции создания, получения, обновления и удаления сделок.
/// </remarks>
[ApiController]
[Route("crm/deals")]
[AuthorizeCrm(GlobalRoles = [SystemRoles.Admin, SystemRoles.Manager, SystemRoles.HeadManager])]
public sealed class DealsController(IDealService dealService) : ControllerBase
{
    /// <summary>
    /// Получает список сделок с учетом фильтров.
    /// </summary>
    /// <param name="request">Параметры фильтрации списка сделок.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция сделок.</returns>
    /// <response code="200">Список сделок успешно получен.</response>
    /// <response code="400">Переданы некорректные параметры фильтрации.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<DealListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyCollection<DealListItemResponse>>> GetAll(
        [FromQuery] GetDealsRequest request,
        CancellationToken cancellationToken)
    {
        var deals = await dealService.GetAllAsync(request, cancellationToken);
        return Ok(deals);
    }

    /// <summary>
    /// Получает сделку по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор сделки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные сделки.</returns>
    /// <response code="200">Сделка найдена.</response>
    /// <response code="404">Сделка не найдена.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DealResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<DealResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deal = await dealService.GetByIdAsync(id, cancellationToken);
        return Ok(deal);
    }

    /// <summary>
    /// Создает новую сделку.
    /// </summary>
    /// <param name="request">Данные сделки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Идентификатор созданной сделки.</returns>
    /// <response code="201">Сделка успешно создана.</response>
    /// <response code="400">Переданы некорректные данные.</response>
    /// <response code="404">Клиент или пользователь не найден.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateDealRequest request,
        CancellationToken cancellationToken)
    {
        var dealId = await dealService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = dealId },
            new
            {
                DealId = dealId,
                Message = "Сделка успешно создана."
            });
    }

    /// <summary>
    /// Обновляет данные сделки.
    /// </summary>
    /// <param name="id">Идентификатор сделки.</param>
    /// <param name="request">Новые данные сделки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешном обновлении.</returns>
    /// <response code="204">Сделка успешно обновлена.</response>
    /// <response code="400">Переданы некорректные данные.</response>
    /// <response code="404">Сделка или пользователь не найден.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateDealRequest request,
        CancellationToken cancellationToken)
    {
        await dealService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Удаляет сделку.
    /// </summary>
    /// <param name="id">Идентификатор сделки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешном удалении.</returns>
    /// <response code="204">Сделка успешно удалена.</response>
    /// <response code="404">Сделка не найдена.</response>
    /// <response code="409">Удаление невозможно из-за связанных сущностей.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await dealService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}