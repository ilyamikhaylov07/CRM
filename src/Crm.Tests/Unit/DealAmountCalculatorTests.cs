using Crm.Application.Common.Exceptions;
using Crm.Application.DealItems;
using Crm.Domain.Entities;
using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Crm.Tests.Unit;

public sealed class DealAmountCalculatorTests
{
    [Fact]
    public async Task RecalculateAsync_DealWithItems_UpdatesPurchaseAmount()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var client = CreateClient();
        var product = CreateProduct();
        var deal = CreateDeal(client);

        dbContext.AddRange(client, product, deal);
        dbContext.DealItems.AddRange(
            CreateDealItem(deal, product, quantity: 2m, unitPrice: 50m),
            CreateDealItem(deal, product, quantity: 1m, unitPrice: 25.5m));
        await dbContext.SaveChangesAsync();

        var calculator = new DealAmountCalculator(
            dbContext,
            NullLogger<DealAmountCalculator>.Instance);

        // Act
        await calculator.RecalculateAsync(deal.Id, CancellationToken.None);

        // Assert
        Assert.Equal(125.5m, deal.PurchaseAmount);
    }

    [Fact]
    public async Task RecalculateAsync_MissingDeal_ThrowsNotFoundException()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var calculator = new DealAmountCalculator(
            dbContext,
            NullLogger<DealAmountCalculator>.Instance);

        // Act
        var act = () => calculator.RecalculateAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(
            act);
    }

    private static CrmDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CrmDbContext(options);
    }

    private static Client CreateClient() => new()
    {
        ExternalId = "client-1",
        Name = "Test Client",
        Age = 30,
        Gender = "Female",
        Location = "Moscow",
        PreviousPurchases = 2,
        FrequencyOfPurchases = "Monthly"
    };

    private static Product CreateProduct() => new()
    {
        Name = "Sneakers",
        Category = "Footwear",
        BasePrice = 50m
    };

    private static Deal CreateDeal(Client client) => new()
    {
        Client = client,
        PurchaseAmount = 0m,
        PurchaseDateUtc = DateTime.UtcNow,
        Season = "Winter",
        PaymentMethod = "Card",
        DiscountApplied = false,
        PromoCodeUsed = false,
        ReviewRating = 4.5m
    };

    private static DealItem CreateDealItem(
        Deal deal,
        Product product,
        decimal quantity,
        decimal unitPrice) => new()
    {
        Deal = deal,
        Product = product,
        Quantity = quantity,
        UnitPrice = unitPrice,
        TotalPrice = quantity * unitPrice
    };
}
