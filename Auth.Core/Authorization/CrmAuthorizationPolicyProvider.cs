using Auth.Core.Authorization.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Auth.Core.Authorization;

/// <summary>
/// Провайдер политик авторизации.
/// </summary>
/// <param name="authorizationOptions"></param>
public sealed class CrmAuthorizationPolicyProvider(
    IOptions<AuthorizationOptions> authorizationOptions
    ) : DefaultAuthorizationPolicyProvider(authorizationOptions)
{
    /// <inheritdoc/>
    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!RolesRequirementParser.TryParse(policyName, PolicyConstants.DefaultPolicyPrefix, out var requirement))
        {
            return base.GetPolicyAsync(policyName);
        }

        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(requirement)
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}
