namespace Auth.Keycloak.Settings;



/// <summary>
/// Настройки проекции ролей Keycloak в стандартные role-claim'ы
/// (<see cref="System.Security.Claims.ClaimTypes.Role"/>).
/// </summary>
public sealed class KeycloakRolesSettings
{
    /// <summary>
    /// Идентификатор клиента, для которого следует извлекать client-роли
    /// из claim'а <c>resource_access</c>.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Указывает, следует ли извлекать realm-роли из claim'а <c>realm_access</c>.
    /// </summary>
    public bool IncludeRealmRoles { get; set; } = true;

    /// <summary>
    /// Указывает, следует ли извлекать client-роли из claim'а <c>resource_access</c>.
    /// </summary>
    public bool IncludeClientRoles { get; set; } = true;

    /// <summary>
    /// Определяет, извлекать ли роли всех клиентов, если <see cref="ClientId"/> не задан.
    /// </summary>
    public bool IncludeAllClientRoles { get; set; } = false;
}
