namespace Crm.Application.Auth.DTOs;

/// <summary>
/// Ответ с токенами авторизации.
/// </summary>
public sealed record LoginResponse
{
    public required string AccessToken { get; init; }

    public required string RefreshToken { get; init; }

    public required int ExpiresIn { get; init; }
}
