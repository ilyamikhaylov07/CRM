using Auth.Keycloak.Constants;
using System.Text.Json;

namespace Auth.Keycloak.Parsing;

/// <summary>
/// Парсер специфичных claim'ов Keycloak, содержащих роли и permissions.
/// </summary>
/// <remarks>
/// Выполняет разбор JSON-структур <c>realm_access</c> и <c>resource_access</c>,
/// а также значения claim'а с permissions.
/// 
/// Все возвращаемые значения:
/// - нормализуются (Trim + ToLowerInvariant);
/// - фильтруются от пустых значений;
/// - возвращаются без дубликатов (без учёта регистра).
/// </remarks>
/// <summary>
/// Парсер специфичных claim'ов Keycloak, содержащих роли.
/// </summary>
/// <remarks>
/// Выполняет разбор JSON-структур <c>realm_access</c> и <c>resource_access</c>.
/// Все возвращаемые значения:
/// <list type="bullet">
/// <item><description>нормализуются через <c>Trim + ToLowerInvariant</c>;</description></item>
/// <item><description>очищаются от пустых значений;</description></item>
/// <item><description>возвращаются без дубликатов.</description></item>
/// </list>
/// </remarks>
internal sealed class KeycloakAccessParser : IKeycloakAccessParser
{
    /// <inheritdoc />
    public IReadOnlyCollection<string> ParseRealmRoles(string json)
    {
        using var document = JsonDocument.Parse(json);

        if (!document.RootElement.TryGetProperty(KeycloakJwtJsonNames.Roles, out var rolesElement) ||
            rolesElement.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var roleElement in rolesElement.EnumerateArray())
        {
            if (roleElement.ValueKind == JsonValueKind.String)
            {
                AddNormalized(roleElement.GetString(), result);
            }
        }

        return result;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> ParseClientRoles(
        string json,
        string? clientId,
        bool includeAllWhenClientIdMissing)
    {
        using var document = JsonDocument.Parse(json);
        var rootElement = document.RootElement;

        if (rootElement.ValueKind != JsonValueKind.Object)
        {
            return [];
        }

        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(clientId))
        {
            if (rootElement.TryGetProperty(clientId, out var clientElement) &&
                clientElement.ValueKind == JsonValueKind.Object)
            {
                AddRolesFromClientObject(clientElement, result);
            }

            return result;
        }

        if (!includeAllWhenClientIdMissing)
        {
            return [];
        }

        foreach (var clientProperty in rootElement.EnumerateObject())
        {
            if (clientProperty.Value.ValueKind == JsonValueKind.Object)
            {
                AddRolesFromClientObject(clientProperty.Value, result);
            }
        }

        return result;
    }

    /// <summary>
    /// Добавляет роли клиента из объекта внутри <c>resource_access</c>.
    /// </summary>
    private static void AddRolesFromClientObject(JsonElement clientElement, HashSet<string> roles)
    {
        if (!clientElement.TryGetProperty(KeycloakJwtJsonNames.Roles, out var rolesElement) ||
            rolesElement.ValueKind != JsonValueKind.Array)
        {
            return;
        }

        foreach (var roleElement in rolesElement.EnumerateArray())
        {
            if (roleElement.ValueKind == JsonValueKind.String)
            {
                AddNormalized(roleElement.GetString(), roles);
            }
        }
    }

    /// <summary>
    /// Нормализует значение и добавляет его в набор, если оно не пустое.
    /// </summary>
    private static void AddNormalized(string? value, HashSet<string> values)
    {
        var normalized = value?.Trim().ToLowerInvariant();

        if (!string.IsNullOrWhiteSpace(normalized))
        {
            values.Add(normalized);
        }
    }
}
