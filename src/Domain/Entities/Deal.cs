using Crm.Domain.Common;

namespace Crm.Domain.Entities
{
    public sealed class Deal : BaseEntity
    {
        public Guid ClientId { get; set; }

        public required Client Client { get; set; }

        public Guid? UserId { get; set; }

        public User? User { get; set; }

        public decimal PurchaseAmount { get; set; }

        public DateTime PurchaseDateUtc { get; set; }

        public required string Season { get; set; }

        public required string PaymentMethod { get; set; }

        public bool DiscountApplied { get; set; }

        public bool PromoCodeUsed { get; set; }

        public decimal ReviewRating { get; set; }

        public string? ShippingType { get; set; }

        public ICollection<DealItem> Items { get; set; } = new List<DealItem>();

        public ICollection<Activity> Activities { get; set; } = new List<Activity>();

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
