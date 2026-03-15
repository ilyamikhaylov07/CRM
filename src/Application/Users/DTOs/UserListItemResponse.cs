namespace Crm.Application.Users.DTOs;

public sealed record UserListItemResponse
{
    public required Guid Id { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required string KeycloakUserId { get; init; }

    public required string Email { get; init; }

    public string? Phone { get; init; }

    public required string Status { get; init; }

    public required Guid RoleId { get; init; }

    public required string RoleName { get; init; }
}