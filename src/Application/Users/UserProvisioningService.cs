using Crm.Application.Common.Exceptions;
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
    IKeycloakAdminService keycloakAdminService,
    ILogger<UserProvisioningService> logger
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
            throw new ConflictException($"Пользователь с email '{request.Email}' уже существует в базе данных.");
        }

        var existingKeycloakUserId = await keycloakAdminService.FindUserIdByEmailAsync(
            request.Email,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(existingKeycloakUserId))
        {
            throw new ConflictException($"Пользователь с email '{request.Email}' уже существует в Keycloak.");
        }

        var role = await dbContext.Roles
             .FirstOrDefaultAsync(x => x.Name == request.RoleName, cancellationToken)
             ?? throw new NotFoundException($"Роль '{request.RoleName}' не найдена.");

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

            logger.LogInformation(
                "Пользователь успешно создан администратором. UserId: {UserId}, KeycloakUserId: {KeycloakUserId}, Role: {Role}",
                user.Id,
                user.KeycloakUserId,
                role.Name);

            return user.Id;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ошибка при создании пользователя администратором. Email: {Email}, Role: {Role}, KeycloakUserId: {KeycloakUserId}",
                request.Email,
                request.RoleName,
                keycloakUserId);

            if (!string.IsNullOrWhiteSpace(keycloakUserId))
            {
                try
                {
                    await keycloakAdminService.DeleteUserAsync(
                        keycloakUserId,
                        cancellationToken);

                    logger.LogWarning(
                        "Выполнена компенсация после ошибки создания пользователя: пользователь удален из Keycloak. Email: {Email}, KeycloakUserId: {KeycloakUserId}",
                        request.Email,
                        keycloakUserId);
                }
                catch (Exception compensationException)
                {
                    logger.LogError(
                        compensationException,
                        "Не удалось выполнить компенсацию после ошибки создания пользователя. Возможен рассинхрон между Keycloak и БД. Email: {Email}, KeycloakUserId: {KeycloakUserId}",
                        request.Email,
                        keycloakUserId);
                }
            }

            throw;
        }
    }
}
