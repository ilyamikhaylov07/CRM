using Auth.Core.Authorization;
using Auth.Core.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Auth.Core.Extensions;

/// <summary>
/// Расширения для регистрации инфраструктуры аутентификации/авторизации.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление основы для аутентификации и авторизации.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="sectionName"></param>
    /// <returns><see cref="IServiceCollection"/> с настроенной auth.</returns>
    public static IServiceCollection AddCrmAuth(
        this IServiceCollection services,
        IConfiguration configuration,
        string? sectionName = null
        )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var scanAuthSettingsSection = configuration.GetSection(sectionName ?? AuthSettings.SectionName);

        services.AddOptions<AuthSettings>()
            .Bind(scanAuthSettingsSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var settings = scanAuthSettingsSection.Get<AuthSettings>() ?? new AuthSettings();

        services.AddAuthentication(auth =>
        {
            auth.DefaultScheme = settings.DefaultScheme;
            auth.DefaultAuthenticateScheme = settings.DefaultScheme;
            auth.DefaultChallengeScheme = settings.DefaultScheme;
            auth.DefaultForbidScheme = settings.DefaultScheme;
        });

        services.AddAuthorization();

        services.AddSingleton<IAuthorizationPolicyProvider, CrmAuthorizationPolicyProvider>();
        services.TryAddEnumerable(ServiceDescriptor.Scoped<IAuthorizationHandler, RolesAuthorizationHandler>());

        return services;
    }
}