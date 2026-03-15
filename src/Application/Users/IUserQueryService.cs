using Crm.Application.Users.DTOs;

namespace Crm.Application.Users;


/// <summary>
/// Сервис чтения пользователей CRM.
/// </summary>
public interface IUserQueryService
{
    /// <summary>
    /// Возвращает список пользователей CRM.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция пользователей.</returns>
    Task<IReadOnlyCollection<UserListItemResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает пользователя CRM по идентификатору.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя CRM.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Модель пользователя или null.</returns>
    Task<UserListItemResponse?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
