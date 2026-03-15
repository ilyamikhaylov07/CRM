namespace Crm.Infrastructure.Keycloak;

/// <summary>
/// Сервис работы с Keycloak Admin API.
/// </summary>
public interface IKeycloakAdminService
{
    /// <summary>
    /// Создаёт пользователя в Keycloak.
    /// </summary>
    /// <param name="email">Email пользователя.</param>
    /// <param name="firstName">Имя пользователя.</param>
    /// <param name="lastName">Фамилия пользователя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Идентификатор пользователя в Keycloak.</returns>
    Task<string> CreateUserAsync(
        string email,
        string firstName,
        string lastName,
        CancellationToken cancellationToken);

    /// <summary>
    /// Назначает пользователю realm-роль.
    /// </summary>
    /// <param name="keycloakUserId">Идентификатор пользователя Keycloak.</param>
    /// <param name="roleName">Имя роли.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task AssignRoleAsync(
        string keycloakUserId,
        string roleName,
        CancellationToken cancellationToken);

    /// <summary>
    /// Снимает роль у пользователя.
    /// </summary>
    /// <param name="keycloakUserId">Идентификатор пользователя Keycloak.</param>
    /// <param name="roleName">Имя роли.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task RemoveRoleAsync(
        string keycloakUserId,
        string roleName,
        CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает роли пользователя Keycloak.
    /// </summary>
    /// <param name="keycloakUserId">Идентификатор пользователя Keycloak.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция ролей пользователя.</returns>
    Task<IReadOnlyCollection<string>> GetUserRolesAsync(
        string keycloakUserId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Изменяет состояние пользователя (enabled / disabled).
    /// </summary>
    /// <param name="keycloakUserId">Идентификатор пользователя Keycloak.</param>
    /// <param name="enabled">Флаг активности пользователя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task SetEnabledAsync(
        string keycloakUserId,
        bool enabled,
        CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет письмо подтверждения email.
    /// </summary>
    /// <param name="keycloakUserId">Идентификатор пользователя Keycloak.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task SendVerifyEmailAsync(
        string keycloakUserId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Отправляет письмо установки пароля.
    /// </summary>
    /// <param name="keycloakUserId">Идентификатор пользователя Keycloak.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task SendSetupPasswordAsync(
        string keycloakUserId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет пользователя из Keycloak.
    /// </summary>
    /// <param name="keycloakUserId">Идентификатор пользователя Keycloak.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task DeleteUserAsync(
        string keycloakUserId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает идентификатор пользователя Keycloak по email, если пользователь существует.
    /// </summary>
    /// <param name="email">Email пользователя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Идентификатор пользователя Keycloak или null.</returns>
    Task<string?> FindUserIdByEmailAsync(
        string email,
        CancellationToken cancellationToken);
}
