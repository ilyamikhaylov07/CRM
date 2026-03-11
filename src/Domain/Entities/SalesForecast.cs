using Crm.Domain.Common;

namespace Crm.Domain.Entities
{
    public sealed class SalesForecast : BaseEntity
    {
        public Guid ClientId { get; set; }

        public required Client Client { get; set; }

        public DateTime ForecastDateUtc { get; set; }

        public DateTime PeriodStartUtc { get; set; }

        public DateTime PeriodEndUtc { get; set; }

        public decimal PredictedAmount { get; set; }

        public decimal? ConfidenceScore { get; set; }

        public string? ModelVersion { get; set; }

        public string? Notes { get; set; }
    }
}
