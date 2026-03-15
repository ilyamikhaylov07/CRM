using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Infrastructure.Database.Configurations;

public sealed class SalesForecastConfiguration : IEntityTypeConfiguration<SalesForecast>
{
    public void Configure(EntityTypeBuilder<SalesForecast> builder)
    {
        builder.ToTable("sales_forecasts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ForecastDateUtc)
            .IsRequired();

        builder.Property(x => x.PeriodStartUtc)
            .IsRequired();

        builder.Property(x => x.PeriodEndUtc)
            .IsRequired();

        builder.Property(x => x.PredictedAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.ConfidenceScore)
            .HasPrecision(5, 4);

        builder.Property(x => x.ModelVersion);

        builder.Property(x => x.Notes);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasOne(x => x.Client)
            .WithMany(x => x.SalesForecasts)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ClientId);
        builder.HasIndex(x => new { x.ClientId, x.ForecastDateUtc });
    }
}
