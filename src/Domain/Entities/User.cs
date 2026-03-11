using Crm.Domain.Common;
using Crm.Domain.Enums;

namespace Crm.Domain.Entities
{
    public sealed class User : BaseEntity
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public string? Phone { get; set; }

        public UserStatus Status { get; set; }

        public Guid RoleId { get; set; }

        public required Role Role { get; set; }

        public ICollection<Deal> Deals { get; set; } = new List<Deal>();

        public ICollection<Activity> Activities { get; set; } = new List<Activity>();

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
