using Crm.Application.Users.DTOs;
using Crm.Domain.Entities;
using Crm.Domain.Enums;
using Crm.Infrastructure.Database;
using Crm.Infrastructure.Keycloak;
using Microsoft.EntityFrameworkCore;

namespace Crm.Application.Users;

/// <summary>
/// Сервис оркестрации создания пользователя в Keycloak и локальной БД.
/// </summary>
public sealed class UserProvisioningService(
    CrmDbContext dbContext,
    IKeycloakAdminService keycloakAdminService
    ) : IUserProvisioningService
{
    /// <inheritdoc />
    public async Task<Guid> CreateByAdminAsync(
        CreateUserByAdminRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existingUserInDb = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

        if (existingUserInDb is not null)
        {
            throw new InvalidOperationException(
                $"Пользователь с email '{request.Email}' уже существует в базе данных.");
        }

        var existingKeycloakUserId = await keycloakAdminService.FindUserIdByEmailAsync(
            request.Email,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(existingKeycloakUserId))
        {
            throw new InvalidOperationException(
                $"Пользователь с email '{request.Email}' уже существует в Keycloak.");
        }

        var role = await dbContext.Roles
            .FirstOrDefaultAsync(x => x.Name == request.RoleName, cancellationToken)
            ?? throw new InvalidOperationException(
                $"Роль '{request.RoleName}' не найдена в базе данных.");

        string? keycloakUserId = null;

        try
        {
            keycloakUserId = await keycloakAdminService.CreateUserAsync(
                request.Email,
                request.FirstName,
                request.LastName,
                cancellationToken);

            await keycloakAdminService.AssignRoleAsync(
                keycloakUserId,
                request.RoleName,
                cancellationToken);

            //await keycloakAdminService.SendVerifyEmailAsync(
            //    keycloakUserId,
            //    cancellationToken);

            //await keycloakAdminService.SendSetupPasswordAsync(
            //    keycloakUserId,
            //    cancellationToken);

            var user = new User
            {
                Id = Guid.NewGuid(),
                KeycloakUserId = keycloakUserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Status = UserStatus.Active,
                RoleId = role.Id,
                Role = role
            };

            await dbContext.Users.AddAsync(user, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(keycloakUserId))
            {
                try
                {
                    await keycloakAdminService.DeleteUserAsync(
                        keycloakUserId,
                        cancellationToken);
                }
                catch
                {
                    // позже можно добавить логирование
                }
            }

            throw;
        }
    }
}
