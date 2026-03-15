using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Infrastructure.Database.Configurations;

public sealed class DealConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder.ToTable("deals");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PurchaseAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.PurchaseDateUtc)
            .IsRequired();

        builder.Property(x => x.Season)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.PaymentMethod)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DiscountApplied)
            .IsRequired();

        builder.Property(x => x.PromoCodeUsed)
            .IsRequired();

        builder.Property(x => x.ReviewRating)
            .HasPrecision(3, 2);

        builder.Property(x => x.ShippingType)
            .HasMaxLength(100);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasOne(x => x.Client)
            .WithMany(x => x.Deals)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Deals)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.PurchaseDateUtc);
        builder.HasIndex(x => x.ClientId);
    }
}
