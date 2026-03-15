namespace Crm.Domain.Constants;

/// <summary>
/// Системные роли CRM.
/// </summary>
public static class SystemRoles
{
    /// <summary>
    /// Администратор системы.
    /// </summary>
    public const string Admin = "admin";

    /// <summary>
    /// Менеджер.
    /// </summary>
    public const string Manager = "manager";

    /// <summary>
    /// Главный менеджер.
    /// </summary>
    public const string HeadManager = "head_manager";

    /// <summary>
    /// Все допустимые системные роли.
    /// </summary>
    public static readonly string[] All =
    [
        Admin,
        Manager,
        HeadManager,
    ];
}
