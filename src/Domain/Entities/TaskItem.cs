using Crm.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;
using TaskStatus = Crm.Domain.Enums.TaskStatus;

namespace Crm.Domain.Entities
{
    public sealed class TaskItem : BaseEntity
    {
        [Column("title")]
        public required string Title { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("due_date_utc")]
        public DateTime DueDateUtc { get; set; }

        [Column("status")]
        public TaskStatus Status { get; set; }

        [Column("assigned_to_user_id")]
        public Guid? AssignedToUserId { get; set; }

        public User? AssignedToUser { get; set; }

        [Column("client_id")]
        public Guid ClientId { get; set; }

        public required Client Client { get; set; }

        [Column("deal_id")]
        public Guid? DealId { get; set; }

        public Deal? Deal { get; set; }
    }
}
