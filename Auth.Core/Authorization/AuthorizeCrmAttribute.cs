using Auth.Core.Authorization.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Auth.Core.Authorization;

/// <summary>
/// Атрибут, формирующий динамическую policy (через имя policy),
/// которую затем построит <see cref="CrmAuthorizationPolicyProvider"/>.
/// </summary>
/// <remarks>
/// <para>
/// Атрибут используется для декларативного задания динамической политики авторизации
/// без ручного формирования строки вида:
/// <code>
/// {prefix}:roles=r1,r2;mode=all|any
/// </code>
/// </para>
/// <para>
/// Префикс фиксирован и задаётся константой
/// <see cref="PolicyConstants.DefaultPolicyPrefix"/>.
/// Его значение должно совпадать с префиксом,
/// используемым в <see cref="CrmAuthorizationPolicyProvider"/>.
/// </para>
/// <para>
/// Должна быть указана хотя бы одна роль.
/// Если оба списка пусты, при построении политики будет выброшено исключение.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class AuthorizeCrmAttribute : Attribute, IAuthorizeData
{
    /// <summary>
    /// Роли, которым разрешён доступ.
    /// </summary>
    public string[] GlobalRoles { get; init; } = [];

    /// <summary>
    /// Требовать все указанные роли.
    /// Если <see langword="false"/>, достаточно любой одной роли.
    /// </summary>
    public bool RequireAllRoles { get; init; }

    /// <inheritdoc/>
    public string? Policy
    {
        get
        {
            var prefix = PolicyConstants.DefaultPolicyPrefix;


            return PolicyNameBuilder.Build(
                prefix: prefix,
                roles: GlobalRoles,
                requireAllRoles: RequireAllRoles);
        }
        set => throw new InvalidOperationException("Свойство Policy не поддерживается для ручного использования.");
    }

    /// <inheritdoc/>
    public string? Roles
    {
        get => null;
        set => throw new InvalidOperationException("Свойство Roles не поддерживается.");
    }

    /// <inheritdoc/>
    public string? AuthenticationSchemes
    {
        get => null;
        set => throw new InvalidOperationException("Свойство AuthenticationSchemes не поддерживается.");
    }
}
