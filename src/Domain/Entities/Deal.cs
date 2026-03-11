using Crm.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Crm.Domain.Entities
{
    public sealed class Deal : BaseEntity
    {
        [Column("client_id")]
        public Guid ClientId { get; set; }

        public required Client Client { get; set; }

        [Column("user_id")]
        public Guid? UserId { get; set; }

        public User? User { get; set; }

        [Column("purchase_amount")]
        public decimal PurchaseAmount { get; set; }

        [Column("purchase_date_utc")]
        public DateTime PurchaseDateUtc { get; set; }

        [Column("season")]
        public required string Season { get; set; }

        [Column("payment_method")]
        public required string PaymentMethod { get; set; }

        [Column("discount_applied")]
        public bool DiscountApplied { get; set; }

        [Column("promo_code_used")]
        public bool PromoCodeUsed { get; set; }

        [Column("review_rating")]
        public decimal ReviewRating { get; set; }

        [Column("shipping_type")]
        public string? ShippingType { get; set; }

        public ICollection<DealItem> Items { get; set; } = new List<DealItem>();

        public ICollection<Activity> Activities { get; set; } = new List<Activity>();

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
