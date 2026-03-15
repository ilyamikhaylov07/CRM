namespace Crm.Application.Users.DTOs;

public sealed record ChangeUserStatusRequest
{
    public required bool IsActive { get; init; }
}
