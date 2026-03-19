using Auth.Keycloak.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Crm.Infrastructure.HealthChecks;

/// <summary>
/// Health check для keycloak.
/// </summary>
public sealed class KeycloakHealthCheck(
    IHttpClientFactory httpClientFactory,
    IOptions<KeycloakJwtSettings> options)
    : IHealthCheck
{
    private readonly KeycloakJwtSettings _options = options.Value;

    /// <summary>
    /// Проверка состояния сервиса.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var authority = _options.Authority;

            if (string.IsNullOrWhiteSpace(authority))
            {
                return HealthCheckResult.Unhealthy("Keycloak Authority не задан.");
            }

            var client = httpClientFactory.CreateClient();

            var response = await client.GetAsync(
                $"{authority}/.well-known/openid-configuration",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Unhealthy(
                    $"Keycloak вернул статус {(int)response.StatusCode}");
            }

            return HealthCheckResult.Healthy("Keycloak доступен.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Keycloak недоступен.", ex);
        }
    }
}