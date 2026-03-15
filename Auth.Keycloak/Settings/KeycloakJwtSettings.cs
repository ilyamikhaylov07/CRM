using System.ComponentModel.DataAnnotations;

namespace Auth.Keycloak.Settings;

/// <summary>
/// Настройки проверки и обработки JWT-токенов, выданных Keycloak.
/// </summary>
/// <remarks>
/// Класс предназначен для биндинга конфигурационной секции
/// <c>Keycloak</c> (см. <see cref="SectionName"/>).
/// Используется при настройке JWT-аутентификации и трансформации ролей.
/// </remarks>
public sealed class KeycloakJwtSettings
{
    /// <summary>
    /// Имя конфигурационной секции в источнике настроек.
    /// </summary>
    /// <remarks>
    /// По умолчанию: Keycloak.
    /// </remarks>
    public const string SectionName = "AuthKeycloak";

    /// <summary>
    /// Адрес сервера авторизации Keycloak.
    /// </summary>
    /// <remarks>
    /// Используется для валидации издателя токена и получения метаданных OpenID Connect.
    /// Значение обязательно и не может быть пустым.
    /// </remarks>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Keycloak:Authority является обязательным параметром.")]
    public required string Authority { get; set; }

    /// <summary>
    /// Ожидаемая аудитория JWT-токена.
    /// </summary>
    /// <remarks>
    /// Если задано, используется для дополнительной проверки claim'а <c>aud</c>.
    /// По умолчанию: <c>null</c> (проверка аудитории не навязывается данным параметром).
    /// </remarks>
    public string? Audience { get; set; }

    /// <summary>
    /// Ожидаемый издатель JWT-токена.
    /// </summary>
    /// <remarks>
    /// Если задано, используется для проверки claim'а <c>iss</c>.
    /// По умолчанию: <c>null</c>.
    /// </remarks>
    public string? Issuer { get; set; }

    /// <summary>
    /// Допустимое расхождение во времени при проверке срока действия токена.
    /// </summary>
    /// <remarks>
    /// Используется при валидации <c>exp</c> и <c>nbf</c>.
    /// Позволяет компенсировать различия системного времени между сервисами.
    /// По умолчанию: 2 минуты.
    /// </remarks>
    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromMinutes(2);

    /// <summary>
    /// Идентификатор клиента Keycloak, используемый при логине.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "AuthKeycloak:ClientId является обязательным параметром.")]
    public required string ClientId { get; set; }

    /// <summary>
    /// Требовать HTTPS для загрузки OIDC metadata.
    /// </summary>
    /// <remarks>
    /// Для production должно оставаться <see langword="true"/>.
    /// Для локальной разработки через HTTP может быть отключено.
    /// </remarks>
    public bool RequireHttpsMetadata { get; set; } = true;

    /// <summary>
    /// Настройки проекции ролей Keycloak в <see cref="System.Security.Claims.ClaimTypes.Role"/>.
    /// </summary>
    /// <remarks>
    /// Определяет, извлекать ли realm-роли, client-роли и каким образом.
    /// По умолчанию: новый экземпляр <see cref="KeycloakRolesSettings"/> с настройками по умолчанию.
    /// </remarks>
    public KeycloakRolesSettings Roles { get; set; } = new();
}
