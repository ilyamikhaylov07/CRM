using Crm.Domain.Common;
using Crm.Domain.Enums;

namespace Crm.Domain.Entities
{
    public sealed class Activity : BaseEntity
    {
        public ActivityType Type { get; set; }

        public string? Subject { get; set; }

        public string? Description { get; set; }

        public DateTime ActivityDateUtc { get; set; }

        public Guid? UserId { get; set; }

        public User? User { get; set; }

        public Guid ClientId { get; set; }

        public required Client Client { get; set; }

        public Guid? DealId { get; set; }

        public Deal? Deal { get; set; }
    }
}
