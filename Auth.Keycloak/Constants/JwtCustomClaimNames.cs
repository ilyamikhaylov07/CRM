namespace Auth.Keycloak.Constants;

/// <summary>
/// Кастомные имена claim's.
/// </summary>
public static class JwtCustomClaimNames
{
    /// <summary>
    /// Идентификатор клиента.
    /// </summary>
    public const string ClientId = "client_id";

    /// <summary>
    /// Идентификатор пользователя в Active Directory.
    /// </summary>
    public const string AdId = "ad_id";

    /// <summary>
    /// Отображаемое имя пользователя.
    /// </summary>
    public const string PreferredUsername = "preferred_username";

    /// <summary>
    /// Роль в организации.
    /// </summary>
    public const string CompanyRole = "company_role";

    /// <summary>
    /// Разрешения.
    /// </summary>
    public const string Permissions = "permissions";
}
