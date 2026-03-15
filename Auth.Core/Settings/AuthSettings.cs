using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.ComponentModel.DataAnnotations;

namespace Auth.Core.Settings;

/// <summary>
/// Настройки инфраструктуры аутентификации и авторизации.
/// </summary>
public sealed class AuthSettings
{
    /// <summary>
    /// Имя секции конфигурации.
    /// </summary>
    /// <remarks>
    /// Значение по умолчанию — <c>"ScanAuth"</c>.
    /// </remarks>
    public const string SectionName = "AuthCore";

    /// <summary>
    /// Схема аутентификации по умолчанию.
    /// </summary>
    /// <remarks>
    /// Значение по умолчанию — <see cref="JwtBearerDefaults.AuthenticationScheme"/> ("Bearer").
    /// </remarks>
    [Required]
    public string DefaultScheme { get; set; } = JwtBearerDefaults.AuthenticationScheme;
}