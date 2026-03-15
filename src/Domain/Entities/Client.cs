using Crm.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Crm.Domain.Entities;

public sealed class Client : BaseEntity
{
    [Column("external_id")]
    public required string ExternalId { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("age")]
    public int Age { get; set; }

    [Column("gender")]
    public required string Gender { get; set; }

    [Column("location")]
    public required string Location { get; set; }

    [Column("previous_purchases")]
    public int PreviousPurchases { get; set; }

    [Column("frequency_of_purchases")]
    public required string FrequencyOfPurchases { get; set; }

    public ICollection<Deal> Deals { get; set; } = new List<Deal>();

    public ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();

    public ICollection<SalesForecast> SalesForecasts { get; set; } = new List<SalesForecast>();

    public ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();
}
