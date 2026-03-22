using Auth.Core.Authorization;
using Crm.Application.DealItems;
using Crm.Application.DealItems.DTOs;
using Crm.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

/// <summary>
/// Контроллер управления позициями сделок.
/// </summary>
/// <remarks>
/// Предоставляет операции создания, получения, обновления и удаления позиций сделки.
/// </remarks>
[ApiController]
[Route("crm/deals/{dealId:guid}/items")]
[AuthorizeCrm(GlobalRoles = [SystemRoles.Admin, SystemRoles.Manager, SystemRoles.HeadManager])]
public sealed class DealItemsController(IDealItemService dealItemService) : ControllerBase
{
    /// <summary>
    /// Получает список позиций сделки.
    /// </summary>
    /// <param name="dealId">Идентификатор сделки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция позиций сделки.</returns>
    /// <response code="200">Список позиций сделки успешно получен.</response>
    /// <response code="404">Сделка не найдена.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<DealItemListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyCollection<DealItemListItemResponse>>> GetAll(
        Guid dealId,
        CancellationToken cancellationToken)
    {
        var items = await dealItemService.GetAllAsync(dealId, cancellationToken);
        return Ok(items);
    }

    /// <summary>
    /// Получает позицию сделки по идентификатору.
    /// </summary>
    /// <param name="dealId">Идентификатор сделки.</param>
    /// <param name="itemId">Идентификатор позиции сделки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные позиции сделки.</returns>
    /// <response code="200">Позиция сделки найдена.</response>
    /// <response code="404">Позиция сделки или сделка не найдена.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpGet("{itemId:guid}")]
    [ProducesResponseType(typeof(DealItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<DealItemResponse>> GetById(
        Guid dealId,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        var item = await dealItemService.GetByIdAsync(dealId, itemId, cancellationToken);
        return Ok(item);
    }

    /// <summary>
    /// Создает новую позицию сделки.
    /// </summary>
    /// <param name="dealId">Идентификатор сделки.</param>
    /// <param name="request">Данные позиции сделки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Идентификатор созданной позиции сделки.</returns>
    /// <response code="201">Позиция сделки успешно создана.</response>
    /// <response code="400">Переданы некорректные данные.</response>
    /// <response code="404">Сделка или товар не найден.</response>
    /// <response code="409">Товар неактивен.</response>
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
        Guid dealId,
        [FromBody] CreateDealItemRequest request,
        CancellationToken cancellationToken)
    {
        var itemId = await dealItemService.CreateAsync(dealId, request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { dealId, itemId },
            new
            {
                DealItemId = itemId,
                Message = "Позиция сделки успешно создана."
            });
    }

    /// <summary>
    /// Обновляет позицию сделки.
    /// </summary>
    /// <param name="dealId">Идентификатор сделки.</param>
    /// <param name="itemId">Идентификатор позиции сделки.</param>
    /// <param name="request">Новые данные позиции сделки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешном обновлении.</returns>
    /// <response code="204">Позиция сделки успешно обновлена.</response>
    /// <response code="400">Переданы некорректные данные.</response>
    /// <response code="404">Сделка, позиция или товар не найдены.</response>
    /// <response code="409">Товар неактивен.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpPut("{itemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(
        Guid dealId,
        Guid itemId,
        [FromBody] UpdateDealItemRequest request,
        CancellationToken cancellationToken)
    {
        await dealItemService.UpdateAsync(dealId, itemId, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Удаляет позицию сделки.
    /// </summary>
    /// <param name="dealId">Идентификатор сделки.</param>
    /// <param name="itemId">Идентификатор позиции сделки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешном удалении.</returns>
    /// <response code="204">Позиция сделки успешно удалена.</response>
    /// <response code="404">Позиция сделки или сделка не найдены.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpDelete("{itemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(
        Guid dealId,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        await dealItemService.DeleteAsync(dealId, itemId, cancellationToken);
        return NoContent();
    }
}