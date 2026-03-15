using Auth.Keycloak.ClaimsTransformers;
using Auth.Keycloak.Parsing;
using Auth.Keycloak.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Auth.Keycloak.Extensions;

/// <summary>
/// Расширения для регистрации JWT-аутентификации Keycloak в DI-контейнере.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует JWT-аутентификацию на основе Keycloak и настраивает
    /// преобразование ролей в стандартные role-claim'ы .NET.
    /// </summary>
    /// <param name="services">Коллекция сервисов приложения.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <param name="schemeName">
    /// Имя схемы аутентификации.
    /// По умолчанию используется <see cref="JwtBearerDefaults.AuthenticationScheme"/>.
    /// </param>
    /// <param name="sectionName">
    /// Имя секции конфигурации с настройками <see cref="KeycloakJwtSettings"/>.
    /// Если не задано, используется <see cref="KeycloakJwtSettings.SectionName"/>.
    /// </param>
    /// <returns>Исходная коллекция <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddKeycloakJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        string schemeName = JwtBearerDefaults.AuthenticationScheme,
        string? sectionName = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var settingsSection = configuration.GetSection(sectionName ?? KeycloakJwtSettings.SectionName);

        services.AddOptions<KeycloakJwtSettings>()
            .Bind(settingsSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IKeycloakAccessParser, KeycloakAccessParser>();
        services.AddSingleton<IClaimsTransformation, KeycloakClaimsTransformer>();

        services.AddAuthentication()
            .AddJwtBearer(schemeName, _ => { });

        services.AddOptions<JwtBearerOptions>(schemeName)
            .Configure<IOptions<KeycloakJwtSettings>>((options, keycloakSettings) =>
            {
                var settings = keycloakSettings.Value;
                var tokenValidationParameters = options.TokenValidationParameters;

                options.Authority = settings.Authority;
                options.RequireHttpsMetadata = settings.RequireHttpsMetadata;
                options.MapInboundClaims = false;

                tokenValidationParameters.ValidateLifetime = true;
                tokenValidationParameters.ClockSkew = settings.ClockSkew;

                tokenValidationParameters.ValidateAudience = !string.IsNullOrWhiteSpace(settings.Audience);
                tokenValidationParameters.ValidAudience = string.IsNullOrWhiteSpace(settings.Audience)
                    ? null
                    : settings.Audience;

                tokenValidationParameters.RoleClaimType = ClaimTypes.Role;

                tokenValidationParameters.ValidateIssuer = true;
                if (!string.IsNullOrWhiteSpace(settings.Issuer))
                {
                    tokenValidationParameters.ValidIssuer = settings.Issuer;
                }
            });

        return services;
    }
}