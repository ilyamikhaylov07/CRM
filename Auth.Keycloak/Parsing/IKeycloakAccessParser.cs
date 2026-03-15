namespace Auth.Keycloak.Parsing;

/// <summary>
/// Парсер ролей из стандартных claim'ов Keycloak.
/// </summary>
public interface IKeycloakAccessParser
{
    /// <summary>
    /// Разбирает realm roles из claim'а <c>realm_access</c>.
    /// </summary>
    IReadOnlyCollection<string> ParseRealmRoles(string json);

    /// <summary>
    /// Разбирает client roles из claim'а <c>resource_access</c>.
    /// </summary>
    IReadOnlyCollection<string> ParseClientRoles(
        string json,
        string? clientId,
        bool includeAllWhenClientIdMissing);
}
