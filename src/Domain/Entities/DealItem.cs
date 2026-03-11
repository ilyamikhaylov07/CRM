using Crm.Domain.Common;

namespace Crm.Domain.Entities
{
    public sealed class DealItem : BaseEntity
    {
        public Guid DealId { get; set; }

        public required Deal Deal { get; set; }

        public Guid ProductId { get; set; }

        public required Product Product { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
