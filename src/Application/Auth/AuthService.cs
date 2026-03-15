using Auth.Keycloak.Settings;
using Crm.Application.Auth.DTOs;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace Crm.Application.Auth;

/// <summary>
/// Сервис аутентификации пользователей через Keycloak.
/// </summary>
public sealed class AuthService(
    IHttpClientFactory httpClientFactory,
    IOptions<KeycloakJwtSettings> settings) : IAuthService
{
    private readonly KeycloakJwtSettings _settings = settings.Value;

    /// <inheritdoc />
    public async Task<LoginResponse?> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Email);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Password);

        var tokenEndpoint = BuildTokenEndpoint(_settings.Authority);

        using var httpClient = httpClientFactory.CreateClient();

        using var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _settings.ClientId,
            ["grant_type"] = "password",
            ["username"] = request.Email,
            ["password"] = request.Password
        });

        var response = await httpClient.PostAsync(
            tokenEndpoint,
            formContent,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(cancellationToken: cancellationToken);

        if (tokenResponse is null)
        {
            throw new InvalidOperationException("Keycloak вернул пустой ответ при получении токена.");
        }

        return new LoginResponse
        {
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            ExpiresIn = tokenResponse.ExpiresIn
        };
    }

    /// <summary>
    /// Строит адрес token endpoint на основе Authority.
    /// </summary>
    /// <param name="authority">Адрес realm в Keycloak.</param>
    /// <returns>Полный адрес token endpoint.</returns>
    private static string BuildTokenEndpoint(string authority)
    {
        return authority.TrimEnd('/') + "/protocol/openid-connect/token";
    }

    /// <summary>
    /// DTO ответа Keycloak token endpoint.
    /// </summary>
    private sealed record KeycloakTokenResponse
    {
        /// <summary>
        /// Access token.
        /// </summary>
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; init; }

        /// <summary>
        /// Refresh token.
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public required string RefreshToken { get; init; }

        /// <summary>
        /// Время жизни access token в секундах.
        /// </summary>
        [JsonPropertyName("expires_in")]
        public required int ExpiresIn { get; init; }
    }
}
