using Auth.Keycloak.Constants;
using Auth.Keycloak.Parsing;
using Auth.Keycloak.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Auth.Keycloak.ClaimsTransformers;

/// <summary>
/// Реализация <see cref="IClaimsTransformation"/>, проецирующая роли
/// из claim'ов JWT-токена Keycloak в стандартные claim'ы приложения.
/// </summary>
/// <remarks>
/// <para>
/// Трансформер работает со стандартными claim'ами Keycloak:
/// </para>
/// <list type="bullet">
/// <item>
/// <description>
/// <c>realm_access</c> — realm roles;
/// </description>
/// </item>
/// <item>
/// <description>
/// <c>resource_access</c> — client roles.
/// </description>
/// </item>
/// </list>
/// <para>
/// Разбор JSON-структур инкапсулирован в <see cref="IKeycloakAccessParser"/>.
/// </para>
/// <para>
/// Дубликаты ролей повторно не добавляются.
/// </para>
/// <para>
/// Ошибки парсинга не должны ронять запрос и логируются на уровне Warning.
/// </para>
/// </remarks>
public sealed class KeycloakClaimsTransformer(
    IOptions<KeycloakJwtSettings> settings,
    IKeycloakAccessParser parser,
    ILogger<KeycloakClaimsTransformer> logger) : IClaimsTransformation
{
    private readonly KeycloakJwtSettings _settings = settings.Value;
    private readonly IKeycloakAccessParser _parser = parser;
    private readonly ILogger<KeycloakClaimsTransformer> _logger = logger;

    /// <summary>
    /// Выполняет трансформацию <see cref="ClaimsPrincipal"/>,
    /// добавляя role claim'ы при необходимости.
    /// </summary>
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity { IsAuthenticated: true } identity)
        {
            return Task.FromResult(principal);
        }

        var rolesSettings = _settings.Roles ?? new KeycloakRolesSettings();

        var existingRoles = new HashSet<string>(
            identity.FindAll(ClaimTypes.Role).Select(c => c.Value),
            StringComparer.OrdinalIgnoreCase);

        if (rolesSettings.IncludeRealmRoles)
        {
            ProjectRealmAccess(identity, existingRoles);
        }

        if (rolesSettings.IncludeClientRoles)
        {
            ProjectResourceAccess(identity, existingRoles, rolesSettings);
        }

        return Task.FromResult(principal);
    }

    /// <summary>
    /// Добавляет realm roles из claim'а <c>realm_access</c>
    /// в виде <see cref="ClaimTypes.Role"/>.
    /// </summary>
    private void ProjectRealmAccess(ClaimsIdentity identity, HashSet<string> existingRoles)
    {
        var json = identity.FindFirst(KeycloakJwtJsonNames.RealmAccess)?.Value;
        if (string.IsNullOrWhiteSpace(json))
        {
            return;
        }

        try
        {
            foreach (var role in _parser.ParseRealmRoles(json))
            {
                if (existingRoles.Add(role))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Возникла ошибка при извлечении realm-ролей из claim '{ClaimType}' Keycloak.",
                KeycloakJwtJsonNames.RealmAccess);
        }
    }

    /// <summary>
    /// Добавляет client roles из claim'а <c>resource_access</c>
    /// в виде <see cref="ClaimTypes.Role"/>.
    /// </summary>
    private void ProjectResourceAccess(
        ClaimsIdentity identity,
        HashSet<string> existingRoles,
        KeycloakRolesSettings rolesSettings)
    {
        if (string.IsNullOrWhiteSpace(rolesSettings.ClientId) &&
            !rolesSettings.IncludeAllClientRoles)
        {
            return;
        }

        var json = identity.FindFirst(KeycloakJwtJsonNames.ResourceAccess)?.Value;
        if (string.IsNullOrWhiteSpace(json))
        {
            return;
        }

        try
        {
            var roles = _parser.ParseClientRoles(
                json,
                rolesSettings.ClientId,
                rolesSettings.IncludeAllClientRoles);

            foreach (var role in roles)
            {
                if (existingRoles.Add(role))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }
        }
        catch (Exception exception)
        {
            var clientIdForLog = string.IsNullOrWhiteSpace(rolesSettings.ClientId)
                ? "<all>"
                : rolesSettings.ClientId;

            _logger.LogWarning(
                exception,
                "Возникла ошибка при извлечении client-ролей клиента '{ClientId}' из claim '{ClaimType}' Keycloak.",
                clientIdForLog,
                KeycloakJwtJsonNames.ResourceAccess);
        }
    }
}
