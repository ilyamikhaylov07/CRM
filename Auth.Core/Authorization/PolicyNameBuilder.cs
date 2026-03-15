using Auth.Core.Authorization.Helpers;
using System.Text;

namespace Auth.Core.Authorization;

/// <summary>
/// Утилита для формирования имени динамической policy для <see cref="CrmAuthorizationPolicyProvider"/>.
/// </summary>
/// <remarks>
/// Формат:
/// <code>
/// {prefix}:roles=r1,r2;mode=all|any
/// </code>
/// Пустые/невалидные значения отфильтровываются.
/// </remarks>
internal static class PolicyNameBuilder
{
    /// <summary>
    /// Собирает имя policy для динамической авторизации по ролям.
    /// </summary>
    /// <param name="prefix">Префикс политики.</param>
    /// <param name="roles">Список ролей.</param>
    /// <param name="requireAllRoles">Требовать ли все роли.</param>
    internal static string Build(
        string prefix,
        IEnumerable<string>? roles = null,
        bool requireAllRoles = false)
    {
        var normalizedRoles = PolicyNormalizer.Normalize(roles);

        if (normalizedRoles.Count == 0)
        {
            throw new ArgumentException("Нельзя сформировать policy: не указаны роли.");
        }

        var sb = new StringBuilder();
        sb.Append(prefix);
        sb.Append(':');

        if (normalizedRoles.Count > 0)
        {
            sb.Append("roles=");
            sb.Append(string.Join(',', normalizedRoles));
            sb.Append(";mode=");
            sb.Append(requireAllRoles ? "all" : "any");
        }

        return sb.ToString();
    }
}
