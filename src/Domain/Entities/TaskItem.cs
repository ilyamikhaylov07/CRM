using Crm.Domain.Common;
using TaskStatus = Crm.Domain.Enums.TaskStatus;

namespace Crm.Domain.Entities
{
    public sealed class TaskItem : BaseEntity
    {
        public required string Title { get; set; }

        public string? Description { get; set; }

        public DateTime DueDateUtc { get; set; }

        public TaskStatus Status { get; set; }

        public Guid AssignedToUserId { get; set; }

        public required User AssignedToUser { get; set; }

        public Guid ClientId { get; set; }

        public required Client Client { get; set; }

        public Guid? DealId { get; set; }

        public Deal? Deal { get; set; }
    }
}
