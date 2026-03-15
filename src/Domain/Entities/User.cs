using Crm.Domain.Common;
using Crm.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Crm.Domain.Entities;

public sealed class User : BaseEntity
{
    [Column("first_name")]
    public required string FirstName { get; set; }

    [Column("last_name")]
    public required string LastName { get; set; }

    [Column("email")]
    public required string Email { get; set; }

    [Column("phone")]
    public string? Phone { get; set; }

    [Column("status")]
    public UserStatus Status { get; set; }

    [Column("role_id")]
    public Guid RoleId { get; set; }

    public required Role Role { get; set; }

    public ICollection<Deal> Deals { get; set; } = new List<Deal>();

    public ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
