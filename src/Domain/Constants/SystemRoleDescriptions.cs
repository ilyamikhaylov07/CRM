namespace Crm.Domain.Constants;

/// <summary>
/// Описания системных ролей CRM.
/// </summary>
public static class SystemRoleDescriptions
{
    /// <summary>
    /// Описание роли администратора.
    /// </summary>
    public const string Admin = "Администратор системы с полным доступом к управлению пользователями и справочниками.";

    /// <summary>
    /// Описание роли менеджера.
    /// </summary>
    public const string Manager = "Менеджер, работающий со сделками, клиентами, задачами и активностями.";

    /// <summary>
    /// Описание роли главного менеджера.
    /// </summary>
    public const string HeadManager = "Главный менеджер, контролирующий работу менеджеров и процессы продаж.";
}