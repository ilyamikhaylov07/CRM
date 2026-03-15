namespace Auth.Core.Authorization.Constants;

/// <summary>
/// Константы, используемые при формировании и разборе динамических политик авторизации.
/// </summary>
internal static class PolicyConstants
{
    /// <summary>
    /// Префикс динамической policy (например: <c>scan:roles=...</c>).
    /// Должен совпадать с префиксом, ожидаемым <see cref="CrmAuthorizationPolicyProvider"/>.
    /// </summary>
    internal const string DefaultPolicyPrefix = "crm";
}
