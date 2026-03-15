using Microsoft.AspNetCore.Authorization;

namespace Auth.Core.Authorization;

/// <summary>
/// Authorization handler, проверяющий требования к ролям пользователя.
/// </summary>
/// Handler используется для обработки <see cref="RolesRequirement"/> и поддерживает:
/// </para>
/// <list type="bullet">
/// <item><description>проверку ролей через <see cref="ClaimsPrincipal.IsInRole"/>;</description></item>
/// <item><description>режимы «все» или «хотя бы одно» для ролей.</description></item>
/// </list>
/// </remarks>
public class RolesAuthorizationHandler : AuthorizationHandler<RolesRequirement>
{

    /// <summary>
    /// Выполняет проверку требований к ролям для текущего пользователя.
    /// </summary>
    /// <param name="context">Контекст авторизации.</param>
    /// <param name="requirement">Требование, содержащее роли и permissions.</param>
    /// <returns>Завершённая задача.</returns>
    /// <remarks>
    /// <para>
    /// Логика проверки:
    /// </para>
    /// <list type="number">
    /// <item><description>если пользователь не аутентифицирован — требование не выполняется;</description></item>
    /// <item><description>если требование пустое (<see cref="RolesRequirement.IsEmpty"/>), оно считается выполненным;</description></item>
    /// <item><description>проверяются роли и permissions согласно флагам <c>RequireAll*</c>;</description></item>
    /// <item><description>при успехе вызывается <see cref="AuthorizationHandlerContext.Succeed"/>.</description></item>
    /// </list>
    /// </remarks>
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return Task.CompletedTask;
        }

        if (requirement.IsEmpty)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var isSatisfied = requirement.RequireAllRoles
            ? requirement.Roles.All(context.User.IsInRole)
            : requirement.Roles.Any(context.User.IsInRole);

        if (isSatisfied)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
