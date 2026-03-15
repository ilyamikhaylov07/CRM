using Auth.Core.Authorization;
using Crm.Application.Users;
using Crm.Application.Users.DTOs;
using Crm.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("crm/users")]
[AuthorizeCrm(GlobalRoles = [SystemRoles.Admin])]
public class UsersController(
    IUserProvisioningService userProvisioningService,
    IUserQueryService userQueryService,
    IUserManagementService userManagementService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<UserListItemResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var users = await userQueryService.GetAllAsync(cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserListItemResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var user = await userQueryService.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return NotFound(new
            {
                ErrorMessage = $"Пользователь '{id}' не найден."
            });
        }

        return Ok(user);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserByAdminRequest request,
        CancellationToken cancellationToken)
    {
        var userId = await userProvisioningService.CreateByAdminAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = userId },
            new
            {
                UserId = userId,
                Message = "Пользователь успешно создан."
            });
    }

    [HttpPatch("{id:guid}/role")]
    public async Task<IActionResult> ChangeRole(
        Guid id,
        [FromBody] ChangeUserRoleRequest request,
        CancellationToken cancellationToken)
    {
        await userManagementService.ChangeRoleAsync(
            id,
            request.RoleName,
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        Guid id,
        [FromBody] ChangeUserStatusRequest request,
        CancellationToken cancellationToken)
    {
        await userManagementService.ChangeStatusAsync(
            id,
            request.IsActive,
            cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/send-verify-email")]
    public async Task<IActionResult> SendVerifyEmail(
        Guid id,
        CancellationToken cancellationToken)
    {
        await userManagementService.SendVerifyEmailAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/send-setup-password")]
    public async Task<IActionResult> SendSetupPassword(
        Guid id,
        CancellationToken cancellationToken)
    {
        await userManagementService.SendSetupPasswordAsync(id, cancellationToken);
        return NoContent();
    }
}
