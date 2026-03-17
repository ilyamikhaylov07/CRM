using Crm.Application.Common.Exceptions;
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
    IKeycloakAdminService keycloakAdminService,
    ILogger<UserManagementService> logger
    ) : IUserManagementService
{
    /// <inheritdoc />
    public async Task ChangeRoleAsync(
        Guid userId,
        string roleName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new ValidationException("Имя роли обязательно.");
        }

        var user = await dbContext.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new NotFoundException($"Пользователь '{userId}' не найден.");

        var oldRoleName = user.Role.Name;

        var newRole = await dbContext.Roles
            .FirstOrDefaultAsync(x => x.Name == roleName, cancellationToken)
            ?? throw new NotFoundException($"Роль '{roleName}' не найдена.");

        if (user.Role.Name == newRole.Name)
        {
            throw new ConflictException(
                $"Пользователю '{userId}' уже назначена роль '{roleName}'.");
        }

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

        logger.LogInformation(
            "Роль пользователя изменена. UserId: {UserId}, KeycloakUserId: {KeycloakUserId}, OldRole: {OldRole}, NewRole: {NewRole}",
            user.Id,
            user.KeycloakUserId,
            oldRoleName,
            newRole.Name);
    }

    /// <inheritdoc />
    public async Task ChangeStatusAsync(
        Guid userId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new NotFoundException($"Пользователь '{userId}' не найден.");

        var newStatus = isActive
            ? UserStatus.Active
            : UserStatus.Inactive;

        if (user.Status == newStatus)
        {
            return;
        }

        await keycloakAdminService.SetEnabledAsync(
            user.KeycloakUserId,
            isActive,
            cancellationToken);

        var oldStatus = user.Status;
        user.Status = newStatus;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Статус пользователя изменен. UserId: {UserId}, KeycloakUserId: {KeycloakUserId}, OldStatus: {OldStatus}, NewStatus: {NewStatus}",
            user.Id,
            user.KeycloakUserId,
            oldStatus,
            newStatus);
    }

    /// <inheritdoc />
    public async Task SendVerifyEmailAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new NotFoundException($"Пользователь '{userId}' не найден.");

        await keycloakAdminService.SendVerifyEmailAsync(
            user.KeycloakUserId,
            cancellationToken);

        logger.LogInformation(
            "Отправлено письмо подтверждения email. UserId: {UserId}, KeycloakUserId: {KeycloakUserId}",
            user.Id,
            user.KeycloakUserId);
    }

    /// <inheritdoc />
    public async Task SendSetupPasswordAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new NotFoundException($"Пользователь '{userId}' не найден.");

        await keycloakAdminService.SendSetupPasswordAsync(
            user.KeycloakUserId,
            cancellationToken);

        logger.LogInformation(
            "Отправлено письмо для первоначальной установки пароля. UserId: {UserId}, KeycloakUserId: {KeycloakUserId}",
            user.Id,
            user.KeycloakUserId);
    }
}
