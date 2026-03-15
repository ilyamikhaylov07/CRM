using Crm.Application.Users.DTOs;
using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Crm.Application.Users;

/// <summary>
/// Сервис чтения пользователей.
/// </summary>
public sealed class UserQueryService(CrmDbContext dbContext) : IUserQueryService
{

    public async Task<IReadOnlyCollection<UserListItemResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Include(x => x.Role)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new UserListItemResponse
            {
                Id = x.Id,
                KeycloakUserId = x.KeycloakUserId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Phone = x.Phone,
                Status = x.Status.ToString(),
                RoleId = x.RoleId,
                RoleName = x.Role.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<UserListItemResponse?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Include(x => x.Role)
            .Where(x => x.Id == userId)
            .Select(x => new UserListItemResponse
            {
                Id = x.Id,
                KeycloakUserId = x.KeycloakUserId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Phone = x.Phone,
                Status = x.Status.ToString(),
                RoleId = x.RoleId,
                RoleName = x.Role.Name
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
