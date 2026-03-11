using Crm.Domain.Common;
using Crm.Domain.Enums;

namespace Crm.Domain.Entities
{
    public sealed class Recommendation : BaseEntity
    {
        public Guid ClientId { get; set; }

        public required Client Client { get; set; }

        public Guid ProductId { get; set; }

        public required Product Product { get; set; }

        public DateTime RecommendationDateUtc { get; set; }

        public decimal Score { get; set; }

        public string? Reason { get; set; }

        public RecommendationStatus Status { get; set; }
    }
}
