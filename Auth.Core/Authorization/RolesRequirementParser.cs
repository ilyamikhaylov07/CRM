namespace Auth.Core.Authorization;

/// <summary>
/// Парсер динамических политик авторизации формата
/// "{prefix}:roles=...;mode=all|any".
/// </summary>
/// <remarks>
/// Используется <see cref="CrmAuthorizationPolicyProvider"/>.
/// Преобразует строковое имя policy в <see cref="RolesRequirement"/>.
/// Возвращает <see langword="false"/>, если формат некорректен
/// или требования отсутствуют.
/// </remarks>
internal static class RolesRequirementParser
{
    private static readonly char SegmentSeparator = ';';
    private static readonly char KeyValueSeparator = '=';

    private static readonly HashSet<string> RoleKeys = 
        new(StringComparer.OrdinalIgnoreCase)
        {
            "roles",
            "role"
        };

    /// <summary>
    /// Пытается распарсить имя динамической политики и сформировать требование ролей.
    /// </summary>
    /// <param name="policyName">Имя политики.</param>
    /// <param name="prefix">Ожидаемый префикс (без двоеточия).</param>
    /// <param name="requirement">Сформированное требование, если разбор успешен.</param>
    /// <returns>
    /// <see langword="true"/>, если имя соответствует формату и удалось извлечь хотя бы одну роль;
    /// иначе <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Имя политики должно начинаться с <c>{prefix}:</c>. Далее идёт “тело” из сегментов, разделённых <c>;</c>.
    /// Сегменты имеют формат <c>key=value</c>. Неизвестные сегменты игнорируются.
    /// </para>
    /// </remarks>
    public static bool TryParse(string policyName, string prefix, out RolesRequirement requirement)
    {
        requirement = null!;

        var pfx = prefix + ":";
        if (!policyName.StartsWith(pfx, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var body = policyName[pfx.Length..];
        if (string.IsNullOrWhiteSpace(body))
        {
            return false;
        }

        var roles = new List<string>();
        var requireAllRoles = false;

        foreach (var segment in body.Split(SegmentSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = segment.Split(KeyValueSeparator, 2, StringSplitOptions.RemoveEmptyEntries);
            if (kv.Length != 2)
            {
                continue;
            }

            var key = kv[0].Trim();
            var value = kv[1].Trim();

            if (RoleKeys.Contains(key))
            {
                roles.AddRange(SplitList(value));
                continue;
            }

            if (key.Equals("mode", StringComparison.OrdinalIgnoreCase))
            {
                requireAllRoles = value.Equals("all", StringComparison.OrdinalIgnoreCase);
            }
        }

        requirement = new RolesRequirement(
            roles: roles,
            requireAllRoles: requireAllRoles);

        return !requirement.IsEmpty;
    }

    private static string[] SplitList(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}

