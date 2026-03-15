using Crm.Application.Auth.DTOs;

namespace Crm.Application.Auth;

/// <summary>
/// Сервис аутентификации пользователей.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Выполняет аутентификацию пользователя через Keycloak.
    /// </summary>
    /// <param name="request">Данные для входа.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Access и refresh токены.</returns>
    Task<LoginResponse?> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);
}
