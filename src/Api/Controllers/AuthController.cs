using Crm.Application.Auth;
using Crm.Application.Auth.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("crm/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Выполняет вход пользователя.
    /// </summary>
    /// <param name="request">Данные для входа.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Токены авторизации.</returns>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);

        if (result == null)
        {
            return BadRequest("Неверный логин или пароль");
        }

        return Ok(result);
    }
}
