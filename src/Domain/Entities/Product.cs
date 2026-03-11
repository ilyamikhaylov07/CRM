using Crm.Domain.Common;

namespace Crm.Domain.Entities
{
    public sealed class Product : BaseEntity
    {
        public required string Name { get; set; }

        public required string Category { get; set; }

        public string? Color { get; set; }

        public string? Size { get; set; }

        public decimal BasePrice { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<DealItem> DealItems { get; set; } = new List<DealItem>();

        public ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();
    }
}
