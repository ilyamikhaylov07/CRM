namespace Crm.Application.Auth.DTOs;

/// <summary>
/// Запрос на получение access token.
/// </summary>
public record class LoginRequest
{
    public required string Email { get; init; }

    public required string Password { get; init; }
}
