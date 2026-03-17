using Crm.Application.Auth;
using Crm.Application.Auth.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;


/// <summary>
/// Контроллер аутентификации пользователей.
/// </summary>
/// <remarks>
/// Предоставляет endpoint для входа пользователя в систему и получения токенов авторизации.
/// </remarks>
[ApiController]
[Route("crm/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Выполняет аутентификацию пользователя по email и паролю.
    /// </summary>
    /// <param name="request">Модель с учетными данными пользователя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Возвращает access token и связанные данные авторизации при успешном входе.
    /// </returns>
    /// <response code="200">Пользователь успешно аутентифицирован.</response>
    /// <response code="400">Переданы неверные учетные данные.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);

        return Ok(result);
    }
}
