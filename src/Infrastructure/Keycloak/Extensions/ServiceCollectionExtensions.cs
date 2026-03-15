using Crm.Infrastructure.Keycloak.Settings;
using Duende.AccessTokenManagement;
using Keycloak.AuthServices.Sdk;

namespace Crm.Infrastructure.Keycloak.Extensions;

/// <summary>
/// Расширения для регистрации Keycloak Admin API.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCrmKeycloakAdmin(
        this IServiceCollection services,
        IConfiguration configuration,
        string? sectionName = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var settingsSection = configuration.GetSection(sectionName ?? KeycloakAdminSettings.SectionName);

        services.AddOptions<KeycloakAdminSettings>()
            .Bind(settingsSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var settings = settingsSection.Get<KeycloakAdminSettings>()
            ?? throw new InvalidOperationException("Секция настроек KeycloakAdmin не найдена.");

        var clientId = configuration["Keycloak:resource"]
            ?? throw new InvalidOperationException(
                "Не задан Keycloak:resource.");

        var clientSecret = configuration["Keycloak:credentials:secret"]
            ?? throw new InvalidOperationException(
                "Не задан Keycloak:credentials:secret.");

        var tokenClientName = ClientCredentialsClientName.Parse("keycloak-admin-token-client");
        var tokenEndpoint = $"{settings.AuthServerUrl.TrimEnd('/')}/realms/{settings.Realm}/protocol/openid-connect/token";

        services.AddHttpClient("keycloak-admin-raw", client =>
        {
            client.BaseAddress = new Uri(settings.AuthServerUrl.TrimEnd('/') + "/");
        })
        .AddClientCredentialsTokenHandler(ClientCredentialsClientName.Parse(tokenClientName));

        services
            .AddClientCredentialsTokenManagement()
            .AddClient(
                tokenClientName,
                client =>
                {
                    client.ClientId = ClientId.Parse(clientId);
                    client.ClientSecret = ClientSecret.Parse(clientSecret);
                    client.TokenEndpoint = new Uri(tokenEndpoint);
                });

        services.AddKeycloakAdminHttpClient(new KeycloakAdminClientOptions
        {
            AuthServerUrl = settings.AuthServerUrl,
            Realm = settings.Realm,
            Resource = settings.Resource
        })
        .AddClientCredentialsTokenHandler(tokenClientName);

        services.AddScoped<IKeycloakAdminService, KeycloakAdminService>();

        return services;
    }
}
