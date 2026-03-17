using Auth.Core.Authorization;
using Crm.Application.Users;
using Crm.Application.Users.DTOs;
using Crm.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

/// <summary>
/// Контроллер управления пользователями.
/// </summary>
/// <remarks>
/// Предоставляет административные операции для просмотра, создания и изменения пользователей.
/// Доступ к методам разрешен только пользователям с глобальной ролью администратора.
/// </remarks>
[ApiController]
[Route("crm/users")]
[AuthorizeCrm(GlobalRoles = [SystemRoles.Admin])]
public class UsersController(
    IUserProvisioningService userProvisioningService,
    IUserQueryService userQueryService,
    IUserManagementService userManagementService) : ControllerBase
{
    /// <summary>
    /// Получает список всех пользователей.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция пользователей системы.</returns>
    /// <response code="200">Список пользователей успешно получен.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<UserListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyCollection<UserListItemResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var users = await userQueryService.GetAllAsync(cancellationToken);
        return Ok(users);
    }

    /// <summary>
    /// Получает пользователя по его идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные пользователя.</returns>
    /// <response code="200">Пользователь найден.</response>
    /// <response code="404">Пользователь с указанным идентификатором не найден.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserListItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserListItemResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var user = await userQueryService.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return NotFound(new
            {
                ErrorMessage = $"Пользователь '{id}' не найден."
            });
        }

        return Ok(user);
    }

    /// <summary>
    /// Создает нового пользователя от имени администратора.
    /// </summary>
    /// <param name="request">Данные для создания пользователя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Идентификатор созданного пользователя и сообщение о результате операции.</returns>
    /// <response code="201">Пользователь успешно создан.</response>
    /// <response code="400">Переданы некорректные данные для создания пользователя.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpPost("create")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserByAdminRequest request,
        CancellationToken cancellationToken)
    {
        var userId = await userProvisioningService.CreateByAdminAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = userId },
            new
            {
                UserId = userId,
                Message = "Пользователь успешно создан."
            });
    }

    /// <summary>
    /// Изменяет роль пользователя.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="request">Новая роль пользователя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешном обновлении роли.</returns>
    /// <response code="204">Роль пользователя успешно изменена.</response>
    /// <response code="400">Переданы некорректные данные для изменения роли.</response>
    /// <response code="404">Пользователь не найден.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpPatch("{id:guid}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ChangeRole(
        Guid id,
        [FromBody] ChangeUserRoleRequest request,
        CancellationToken cancellationToken)
    {
        await userManagementService.ChangeRoleAsync(
            id,
            request.RoleName,
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Изменяет статус активности пользователя.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="request">Новый статус активности пользователя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешном обновлении статуса.</returns>
    /// <response code="204">Статус пользователя успешно изменен.</response>
    /// <response code="400">Переданы некорректные данные для изменения статуса.</response>
    /// <response code="404">Пользователь не найден.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ChangeStatus(
        Guid id,
        [FromBody] ChangeUserStatusRequest request,
        CancellationToken cancellationToken)
    {
        await userManagementService.ChangeStatusAsync(
            id,
            request.IsActive,
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Отправляет пользователю письмо для подтверждения email.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешной отправке письма.</returns>
    /// <response code="204">Письмо для подтверждения email успешно отправлено.</response>
    /// <response code="404">Пользователь не найден.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpPost("{id:guid}/send-verify-email")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SendVerifyEmail(
        Guid id,
        CancellationToken cancellationToken)
    {
        await userManagementService.SendVerifyEmailAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Отправляет пользователю письмо для первоначальной установки пароля.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешной отправке письма.</returns>
    /// <response code="204">Письмо для установки пароля успешно отправлено.</response>
    /// <response code="404">Пользователь не найден.</response>
    /// <response code="401">Пользователь не аутентифицирован.</response>
    /// <response code="403">Недостаточно прав для выполнения операции.</response>
    [HttpPost("{id:guid}/send-setup-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SendSetupPassword(
        Guid id,
        CancellationToken cancellationToken)
    {
        await userManagementService.SendSetupPasswordAsync(id, cancellationToken);
        return NoContent();
    }
}