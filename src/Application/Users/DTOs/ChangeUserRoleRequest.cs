namespace Crm.Application.Users.DTOs;

public sealed record ChangeUserRoleRequest
{
    public required string RoleName { get; init; }
}
