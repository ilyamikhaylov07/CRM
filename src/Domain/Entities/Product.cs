using Crm.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Crm.Domain.Entities;

public sealed class Product : BaseEntity
{
    [Column("name")]
    public required string Name { get; set; }

    [Column("category")]
    public required string Category { get; set; }

    [Column("color")]
    public string? Color { get; set; }

    [Column("size")]
    public string? Size { get; set; }

    [Column("base_price")]
    public decimal BasePrice { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    public ICollection<DealItem> DealItems { get; set; } = new List<DealItem>();

    public ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();
}
