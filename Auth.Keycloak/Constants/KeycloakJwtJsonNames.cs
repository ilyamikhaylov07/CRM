namespace Auth.Keycloak.Constants;

/// <summary>
/// Константы имён стандартных JSON-секций и свойств Keycloak в JWT.
/// </summary>
public static class KeycloakJwtJsonNames
{
    /// <summary>
    /// Секция доступа к клиентам (client roles).
    /// </summary>
    public const string ResourceAccess = "resource_access";

    /// <summary>
    /// Секция realm-ролей.
    /// </summary>
    public const string RealmAccess = "realm_access";

    /// <summary>
    /// Имя свойства с ролями внутри секций доступа.
    /// </summary>
    public const string Roles = "roles";
}
