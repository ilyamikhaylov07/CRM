using Crm.Domain.Constants;
using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Crm.Infrastructure.Database;

/// <summary>
/// Инициализация системных ролей CRM.
/// </summary>
public sealed class RoleSeeder(CrmDbContext dbContext)
{
    private readonly CrmDbContext _dbContext = dbContext;

    /// <summary>
    /// Создаёт системные роли, если они отсутствуют.
    /// </summary>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var existingRoles = await _dbContext.Roles
            .Select(r => r.Name)
            .ToListAsync(cancellationToken);

        var rolesToCreate = new List<Role>();

        if (!existingRoles.Contains(SystemRoles.Admin))
        {
            rolesToCreate.Add(new Role
            {
                Id = Guid.NewGuid(),
                Name = SystemRoles.Admin,
                Description = SystemRoleDescriptions.Admin
            });
        }

        if (!existingRoles.Contains(SystemRoles.Manager))
        {
            rolesToCreate.Add(new Role
            {
                Id = Guid.NewGuid(),
                Name = SystemRoles.Manager,
                Description = SystemRoleDescriptions.Manager
            });
        }

        if (!existingRoles.Contains(SystemRoles.HeadManager))
        {
            rolesToCreate.Add(new Role
            {
                Id = Guid.NewGuid(),
                Name = SystemRoles.HeadManager,
                Description = SystemRoleDescriptions.HeadManager
            });
        }

        if (rolesToCreate.Count == 0)
        {
            return;
        }

        await _dbContext.Roles.AddRangeAsync(rolesToCreate, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
