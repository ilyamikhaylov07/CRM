using Crm.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Crm.Domain.Entities
{
    public sealed class DealItem : BaseEntity
    {
        [Column("deal_id")]
        public Guid DealId { get; set; }

        public required Deal Deal { get; set; }

        [Column("product_id")]
        public Guid ProductId { get; set; }

        public required Product Product { get; set; }

        [Column("quantity")]
        public decimal Quantity { get; set; }

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("total_price")]
        public decimal TotalPrice { get; set; }
    }
}
