using Crm.Domain.Enums;
using Crm.Infrastructure.Database;
using Crm.Infrastructure.Keycloak;
using Microsoft.EntityFrameworkCore;

namespace Crm.Application.Users;

/// <summary>
/// Сервис управления существующими пользователями.
/// </summary>
public sealed class UserManagementService(
    CrmDbContext dbContext,
    IKeycloakAdminService keycloakAdminService
    ) : IUserManagementService
{
    public async Task ChangeRoleAsync(
        Guid userId,
        string roleName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(roleName);

        var user = await dbContext.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException($"Пользователь '{userId}' не найден.");

        var newRole = await dbContext.Roles
            .FirstOrDefaultAsync(x => x.Name == roleName, cancellationToken)
            ?? throw new InvalidOperationException($"Роль '{roleName}' не найдена.");

        var currentRoles = await keycloakAdminService.GetUserRolesAsync(
            user.KeycloakUserId,
            cancellationToken);

        foreach (var currentRole in currentRoles)
        {
            await keycloakAdminService.RemoveRoleAsync(
                user.KeycloakUserId,
                currentRole,
                cancellationToken);
        }

        await keycloakAdminService.AssignRoleAsync(
            user.KeycloakUserId,
            newRole.Name,
            cancellationToken);

        user.RoleId = newRole.Id;
        user.Role = newRole;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(
        Guid userId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException($"Пользователь '{userId}' не найден.");

        var status = isActive ? UserStatus.Active : UserStatus.Inactive;

        await keycloakAdminService.SetEnabledAsync(
            user.KeycloakUserId,
            isActive,
            cancellationToken);

        user.Status = status;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SendVerifyEmailAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException($"Пользователь '{userId}' не найден.");

        await keycloakAdminService.SendVerifyEmailAsync(
            user.KeycloakUserId,
            cancellationToken);
    }

    public async Task SendSetupPasswordAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException($"Пользователь '{userId}' не найден.");

        await keycloakAdminService.SendSetupPasswordAsync(
            user.KeycloakUserId,
            cancellationToken);
    }
}
