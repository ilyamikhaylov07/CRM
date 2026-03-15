using Crm.Application.Users.DTOs;

namespace Crm.Application.Users;


/// <summary>
/// Сервис создания пользователей.
/// </summary>
public interface IUserProvisioningService
{
    /// <summary>
    /// Создаёт пользователя администратором.
    /// </summary>
    Task<Guid> CreateByAdminAsync(CreateUserByAdminRequest request, CancellationToken cancellationToken = default);
}
