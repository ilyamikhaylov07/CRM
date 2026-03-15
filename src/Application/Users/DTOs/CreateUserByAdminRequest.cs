namespace Crm.Application.Users.DTOs;


/// <summary>
/// Запрос на создание пользователя администратором.
/// </summary>
public sealed record CreateUserByAdminRequest
{
    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// Фамилия пользователя.
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    /// Email пользователя.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Телефон пользователя.
    /// </summary>
    public string? Phone { get; init; }

    /// <summary>
    /// Имя роли.
    /// </summary>
    public required string RoleName { get; init; }
}
