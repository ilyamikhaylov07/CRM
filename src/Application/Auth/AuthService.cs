using Auth.Keycloak.Settings;
using Crm.Application.Auth.DTOs;
using Crm.Application.Common.Exceptions;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace Crm.Application.Auth;

/// <summary>
/// Сервис аутентификации пользователей через Keycloak.
/// </summary>
public sealed class AuthService(
    IHttpClientFactory httpClientFactory,
    IOptions<KeycloakJwtSettings> settings,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly KeycloakJwtSettings _settings = settings.Value;

    /// <inheritdoc />
    public async Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ValidationException("Не переданы данные для входа.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ValidationException("Email обязателен.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ValidationException("Пароль обязателен.");
        }

        var tokenEndpoint = BuildTokenEndpoint(_settings.Authority);
        var emailMasked = MaskEmail(request.Email);

        using var httpClient = httpClientFactory.CreateClient();

        using var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _settings.ClientId,
            ["grant_type"] = "password",
            ["username"] = request.Email,
            ["password"] = request.Password
        });

        HttpResponseMessage response;

        try
        {
            response = await httpClient.PostAsync(
                tokenEndpoint,
                formContent,
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ошибка обращения к Keycloak при аутентификации. Authority: {Authority}",
                _settings.Authority);
            throw new ExternalServiceException(
                "Не удалось выполнить запрос к Keycloak для получения токена.",
                exception);
        }

        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
            response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            logger.LogWarning(
                "Неуспешная попытка входа. Email: {EmailMasked}",
                emailMasked);
            throw new AuthenticationException("Неверный логин или пароль.");
        }

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError(
                "Keycloak вернул неуспешный статус при аутентификации. StatusCode: {StatusCode}, Authority: {Authority}",
                (int)response.StatusCode,
                _settings.Authority);

            throw new ExternalServiceException(
                $"Keycloak вернул ошибку при получении токена. Код ответа: {(int)response.StatusCode}.");
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(
            cancellationToken: cancellationToken);

        if (tokenResponse is null)
        {
            logger.LogError(
                "Keycloak вернул пустой ответ при аутентификации. Authority: {Authority}",
                _settings.Authority);

            throw new ExternalServiceException(
                "Keycloak вернул пустой ответ при получении токена.");
        }

        logger.LogInformation(
            "Пользователь успешно аутентифицирован. Email: {EmailMasked}",
            emailMasked);

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
    /// Маскирует email для безопасного логирования.
    /// </summary>
    private static string MaskEmail(string email)
    {
        var atIndex = email.IndexOf('@');

        if (atIndex <= 1)
        {
            return "***";
        }

        var namePart = email[..atIndex];
        var domainPart = email[atIndex..];

        return $"{namePart[0]}***{domainPart}";
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
