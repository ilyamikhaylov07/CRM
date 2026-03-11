using Crm.Domain.Common;

namespace Crm.Domain.Entities
{
    public sealed class Client : BaseEntity
    {
        public required string ExternalId { get; set; }

        public required string Name { get; set; }

        public int Age { get; set; }

        public required string Gender { get; set; }

        public required string Location { get; set; }

        public int PreviousPurchases { get; set; }

        public required string FrequencyOfPurchases { get; set; }

        public ICollection<Deal> Deals { get; set; } = new List<Deal>();

        public ICollection<Activity> Activities { get; set; } = new List<Activity>();

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();

        public ICollection<SalesForecast> SalesForecasts { get; set; } = new List<SalesForecast>();

        public ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();
    }
}
