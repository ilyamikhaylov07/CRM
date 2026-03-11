using Crm.Domain.Common;
using Crm.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Crm.Domain.Entities
{
    public sealed class Recommendation : BaseEntity
    {
        [Column("client_id")]
        public Guid ClientId { get; set; }

        public required Client Client { get; set; }

        [Column("product_id")]
        public Guid ProductId { get; set; }

        public required Product Product { get; set; }

        [Column("recommendation_date_utc")]
        public DateTime RecommendationDateUtc { get; set; }

        [Column("score")]
        public decimal Score { get; set; }

        [Column("reason")]
        public string? Reason { get; set; }

        [Column("status")]
        public RecommendationStatus Status { get; set; }
    }
}
