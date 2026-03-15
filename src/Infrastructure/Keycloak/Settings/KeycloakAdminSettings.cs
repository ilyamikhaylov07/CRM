using System.ComponentModel.DataAnnotations;

namespace Crm.Infrastructure.Keycloak.Settings;

/// <summary>
/// Настройки подключения к Keycloak Admin API.
/// </summary>
public sealed class KeycloakAdminSettings
{
    /// <summary>
    /// Имя секции конфигурации.
    /// </summary>
    public const string SectionName = "KeycloakAdmin";

    /// <summary>
    /// Базовый адрес Keycloak.
    /// Например: http://localhost:8080/
    /// </summary>
    [Required]
    public required string AuthServerUrl { get; set; }

    /// <summary>
    /// Realm, в котором будет выполняться работа через Admin API.
    /// Для CRM это обычно realm crm.
    /// </summary>
    [Required]
    public required string Realm { get; set; }

    /// <summary>
    /// Имя admin client resource.
    /// </summary>
    [Required]
    public required string Resource { get; set; }
}