using Crm.Domain.Common;
using Crm.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Crm.Domain.Entities
{
    public sealed class Activity : BaseEntity
    {
        [Column("type")]
        public ActivityType Type { get; set; }

        [Column("subject")]
        public string? Subject { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("activity_date_utc")]
        public DateTime ActivityDateUtc { get; set; }

        [Column("user_id")]
        public Guid? UserId { get; set; }

        public User? User { get; set; }

        [Column("client_id")]
        public Guid ClientId { get; set; }

        public required Client Client { get; set; }

        [Column("deal_id")]
        public Guid? DealId { get; set; }

        public Deal? Deal { get; set; }
    }
}
