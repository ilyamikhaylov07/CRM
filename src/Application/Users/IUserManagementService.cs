namespace Crm.Application.Users;

/// <summary>
/// Сервис управления существующими пользователями CRM.
/// </summary>
public interface IUserManagementService
{
    /// <summary>
    /// Изменяет роль пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя CRM.</param>
    /// <param name="roleName">Имя новой роли.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task ChangeRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Изменяет статус пользователя (активен / заблокирован).
    /// </summary>
    /// <param name="userId">Идентификатор пользователя CRM.</param>
    /// <param name="isActive">Флаг активности пользователя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task ChangeStatusAsync(Guid userId, bool isActive, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправляет письмо подтверждения email.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя CRM.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task SendVerifyEmailAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправляет письмо установки пароля.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя CRM.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task SendSetupPasswordAsync(Guid userId, CancellationToken cancellationToken = default);
}
