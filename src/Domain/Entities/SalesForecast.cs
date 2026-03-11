using Crm.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Crm.Domain.Entities
{
    public sealed class SalesForecast : BaseEntity
    {
        [Column("client_id")]
        public Guid ClientId { get; set; }

        public required Client Client { get; set; }

        [Column("forecast_date_utc")]
        public DateTime ForecastDateUtc { get; set; }

        [Column("period_start_utc")]
        public DateTime PeriodStartUtc { get; set; }

        [Column("period_end_utc")]
        public DateTime PeriodEndUtc { get; set; }

        [Column("predicted_amount")]
        public decimal PredictedAmount { get; set; }

        [Column("confidence_score")]
        public decimal? ConfidenceScore { get; set; }

        [Column("model_version")]
        public string? ModelVersion { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }
    }
}
