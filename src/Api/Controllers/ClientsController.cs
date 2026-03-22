using Auth.Core.Authorization;
using Crm.Application.Clients;
using Crm.Application.Clients.DTOs;
using Crm.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

/// <summary>
/// Контроллер управления клиентами.
/// </summary>
/// <remarks>
/// Предоставляет операции создания, получения, обновления и удаления клиентов.
/// </remarks>
[ApiController]
[Route("crm/clients")]
[AuthorizeCrm(GlobalRoles = [SystemRoles.Admin, SystemRoles.Manager, SystemRoles.HeadManager])]
public sealed class ClientsController(IClientService clientService) : ControllerBase
{
    /// <summary>
    /// Получает список клиентов с учетом фильтров.
    /// </summary>
    /// <param name="request">Параметры фильтрации списка клиентов.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция клиентов.</returns>
    /// <response code="200">Список клиентов успешно получен.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ClientListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyCollection<ClientListItemResponse>>> GetAll(
        [FromQuery] GetClientsRequest request,
        CancellationToken cancellationToken)
    {
        var clients = await clientService.GetAllAsync(request, cancellationToken);
        return Ok(clients);
    }

    /// <summary>
    /// Получает клиента по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор клиента.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные клиента.</returns>
    /// <response code="200">Клиент найден.</response>
    /// <response code="404">Клиент не найден.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ClientResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var client = await clientService.GetByIdAsync(id, cancellationToken);
        return Ok(client);
    }

    /// <summary>
    /// Создает нового клиента.
    /// </summary>
    /// <param name="request">Данные клиента.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Идентификатор созданного клиента.</returns>
    /// <response code="201">Клиент успешно создан.</response>
    /// <response code="400">Переданы некорректные данные.</response>
    /// <response code="409">Клиент с таким внешним идентификатором уже существует.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateClientRequest request,
        CancellationToken cancellationToken)
    {
        var clientId = await clientService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = clientId },
            new
            {
                ClientId = clientId,
                Message = "Клиент успешно создан."
            });
    }

    /// <summary>
    /// Обновляет данные клиента.
    /// </summary>
    /// <param name="id">Идентификатор клиента.</param>
    /// <param name="request">Новые данные клиента.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешном обновлении.</returns>
    /// <response code="204">Клиент успешно обновлен.</response>
    /// <response code="400">Переданы некорректные данные.</response>
    /// <response code="404">Клиент не найден.</response>
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
        [FromBody] UpdateClientRequest request,
        CancellationToken cancellationToken)
    {
        await clientService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Удаляет клиента.
    /// </summary>
    /// <param name="id">Идентификатор клиента.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешном удалении.</returns>
    /// <response code="204">Клиент успешно удален.</response>
    /// <response code="404">Клиент не найден.</response>
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
        await clientService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}