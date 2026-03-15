using Auth.Core.Authorization.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Auth.Core.Authorization;

/// <summary>
/// Требование авторизации, описывающее набор ролей,
/// необходимых для доступа к ресурсу.
/// </summary>
/// <param name="roles">
/// Набор ролей, требуемых для доступа. Может быть <see langword="null"/>.
/// Пустые и пробельные значения будут отброшены, сравнение выполняется без учёта регистра.
/// </param>
/// <param name="requireAllRoles">
/// Если <see langword="true"/>, пользователь должен иметь все указанные роли;
/// иначе достаточно хотя бы одной.
/// </param>
/// <remarks>
/// Требование поддерживает одновременную проверку ролей.
/// </remarks>
public sealed class RolesRequirement(
    IEnumerable<string?>? roles,
    bool requireAllRoles
    ) : IAuthorizationRequirement
{
    /// <summary>
    /// Набор ролей, необходимых для доступа.
    /// </summary>
    public IReadOnlyCollection<string> Roles { get; } = PolicyNormalizer.Normalize(roles);

    /// <summary>
    /// Требовать наличия всех указанных ролей.
    /// Если <see langword="false"/>, достаточно хотя бы одной роли.
    /// </summary>
    public bool RequireAllRoles { get; } = requireAllRoles;

    /// <summary>
    /// Признак того, что требование не содержит ролей.
    /// </summary>
    /// <remarks>
    /// Используется обработчиком как оптимизация:
    /// пустое требование считается автоматически выполненным
    /// для аутентифицированного пользователя.
    /// </remarks>
    public bool IsEmpty => Roles.Count == 0;
}
