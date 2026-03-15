using Crm.Infrastructure.Keycloak.Models;
using Crm.Infrastructure.Keycloak.Settings;
using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;
using Keycloak.AuthServices.Sdk.Admin.Requests.Users;
using Microsoft.Extensions.Options;

namespace Crm.Infrastructure.Keycloak;

/// <summary>
/// Реализация адаптера над Keycloak Admin API.
/// </summary>
public sealed class KeycloakAdminService(
    IKeycloakUserClient userClient,
    IHttpClientFactory httpClientFactory,
    IOptions<KeycloakAdminSettings> settings,
    ILogger<KeycloakAdminService> logger
    ) : IKeycloakAdminService
{
    private readonly KeycloakAdminSettings _settings = settings.Value;

    public async Task<string> CreateUserAsync(
        string email,
        string firstName,
        string lastName,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        var user = new UserRepresentation
        {
            Username = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Enabled = true,
            EmailVerified = false
        };

        await userClient.CreateUserAsync(_settings.Realm, user, cancellationToken);

        var createdUser = await FindUserByUsernameOrThrowAsync(email, cancellationToken);

        logger.LogInformation(
            "Пользователь {Email} создан в Keycloak с id {KeycloakUserId}.",
            email,
            createdUser.Id);

        return createdUser.Id
            ?? throw new InvalidOperationException("Keycloak вернул пользователя без идентификатора.");
    }

    public async Task AssignRoleAsync(
        string keycloakUserId,
        string roleName,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keycloakUserId);
        ArgumentException.ThrowIfNullOrWhiteSpace(roleName);

        var httpClient = httpClientFactory.CreateClient("keycloak-admin-raw");
        var role = await FindRealmRoleByNameAsync(httpClient, roleName, cancellationToken);

        var requestUri = $"admin/realms/{_settings.Realm}/users/{keycloakUserId}/role-mappings/realm";

        var response = await httpClient.PostAsJsonAsync(
            requestUri,
            new[] { role },
            cancellationToken);

        response.EnsureSuccessStatusCode();

        logger.LogInformation(
            "Пользователю {KeycloakUserId} назначена роль {RoleName} в Keycloak.",
            keycloakUserId,
            roleName);
    }

    public async Task RemoveRoleAsync(
        string keycloakUserId,
        string roleName,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keycloakUserId);
        ArgumentException.ThrowIfNullOrWhiteSpace(roleName);

        var httpClient = httpClientFactory.CreateClient("keycloak-admin-raw");
        var role = await FindRealmRoleByNameAsync(httpClient, roleName, cancellationToken);

        var requestUri = $"admin/realms/{_settings.Realm}/users/{keycloakUserId}/role-mappings/realm";

        using var request = new HttpRequestMessage(HttpMethod.Delete, requestUri)
        {
            Content = JsonContent.Create(new[] { role })
        };

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        logger.LogInformation(
            "У пользователя {KeycloakUserId} снята роль {RoleName} в Keycloak.",
            keycloakUserId,
            roleName);
    }

    public async Task<IReadOnlyCollection<string>> GetUserRolesAsync(
        string keycloakUserId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keycloakUserId);

        var httpClient = httpClientFactory.CreateClient("keycloak-admin-raw");
        var requestUri = $"admin/realms/{_settings.Realm}/users/{keycloakUserId}/role-mappings/realm";

        var roles = await httpClient.GetFromJsonAsync<List<KeycloakRoleRepresentation>>(
            requestUri,
            cancellationToken);

        return roles?.Select(x => x.Name).ToArray() ?? [];
    }

    public async Task SetEnabledAsync(
        string keycloakUserId,
        bool enabled,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keycloakUserId);

        var user = await userClient.GetUserAsync(
            _settings.Realm,
            keycloakUserId,
            cancellationToken: cancellationToken);

        user.Enabled = enabled;

        await userClient.UpdateUserAsync(
            _settings.Realm,
            keycloakUserId,
            user,
            cancellationToken);

        logger.LogInformation(
            "Пользователь {KeycloakUserId} в Keycloak переведён в состояние Enabled={Enabled}.",
            keycloakUserId,
            enabled);
    }

    public async Task SendVerifyEmailAsync(
        string keycloakUserId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keycloakUserId);

        await userClient.SendVerifyEmailAsync(
            _settings.Realm,
            keycloakUserId,
            cancellationToken: cancellationToken);
    }

    public async Task SendSetupPasswordAsync(
        string keycloakUserId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keycloakUserId);

        var request = new ExecuteActionsEmailRequest
        {
            Actions = ["UPDATE_PASSWORD"]
        };

        await userClient.ExecuteActionsEmailAsync(
            _settings.Realm,
            keycloakUserId,
            request,
            cancellationToken);
    }

    public async Task DeleteUserAsync(
        string keycloakUserId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keycloakUserId);

        await userClient.DeleteUserAsync(
            _settings.Realm,
            keycloakUserId,
            cancellationToken);

        logger.LogWarning(
            "Пользователь {KeycloakUserId} удалён из Keycloak.",
            keycloakUserId);
    }

    public async Task<string?> FindUserIdByEmailAsync(
    string email,
    CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var users = await userClient.GetUsersAsync(
            _settings.Realm,
            new GetUsersRequestParameters
            {
                Email = email,
                Exact = true
            },
            cancellationToken);

        var user = users.SingleOrDefault();

        return user?.Id;
    }

    private async Task<UserRepresentation> FindUserByUsernameOrThrowAsync(
        string username,
        CancellationToken cancellationToken)
    {
        var users = await userClient.GetUsersAsync(
            _settings.Realm,
            new GetUsersRequestParameters
            {
                Username = username,
                Exact = true
            },
            cancellationToken);

        var user = users.SingleOrDefault();

        if (user is null)
        {
            throw new InvalidOperationException(
                $"Не удалось найти пользователя '{username}' в Keycloak после создания.");
        }

        return user;
    }

    private async Task<KeycloakRoleRepresentation> FindRealmRoleByNameAsync(
        HttpClient httpClient,
        string roleName,
        CancellationToken cancellationToken)
    {
        var requestUri = $"admin/realms/{_settings.Realm}/roles/{Uri.EscapeDataString(roleName)}";

        var role = await httpClient.GetFromJsonAsync<KeycloakRoleRepresentation>(
            requestUri,
            cancellationToken);

        if (role is null)
        {
            throw new InvalidOperationException(
                $"Не удалось получить realm-роль '{roleName}' из Keycloak.");
        }

        return role;
    }
}
