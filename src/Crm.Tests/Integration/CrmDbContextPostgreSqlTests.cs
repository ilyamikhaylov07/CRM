using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Crm.Tests.Integration;

public sealed class CrmDbContextPostgreSqlTests(PostgreSqlDatabaseFixture fixture)
    : IClassFixture<PostgreSqlDatabaseFixture>
{
    [Fact]
    public async Task SaveChangesAsync_DealWithItems_PersistsGraphInPostgreSql()
    {
        // Arrange
        await using var dbContext = fixture.CreateDbContext();
        await dbContext.Database.EnsureCreatedAsync();
        var client = CreateClient("client-pg-1");
        var product = CreateProduct();
        var deal = CreateDeal(client);

        dbContext.AddRange(client, product, deal);
        dbContext.DealItems.Add(CreateDealItem(deal, product, 3m, 40m));

        // Act
        await dbContext.SaveChangesAsync();

        // Assert
        var savedDeal = await dbContext.Deals
            .Include(x => x.Client)
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .SingleAsync(x => x.Id == deal.Id);

        Assert.Equal("client-pg-1", savedDeal.Client.ExternalId);
        Assert.Single(savedDeal.Items);
        Assert.Equal(120m, savedDeal.Items.Single().TotalPrice);
    }

    [Fact]
    public async Task SaveChangesAsync_ProductWithDealItem_ThrowsDbUpdateException()
    {
        // Arrange
        await using var seedDbContext = fixture.CreateDbContext();
        await seedDbContext.Database.EnsureCreatedAsync();
        var client = CreateClient("client-pg-2");
        var product = CreateProduct();
        var deal = CreateDeal(client);

        seedDbContext.AddRange(client, product, deal);
        seedDbContext.DealItems.Add(CreateDealItem(deal, product, 1m, 40m));
        await seedDbContext.SaveChangesAsync();

        await using var dbContext = fixture.CreateDbContext();
        product = await dbContext.Products.SingleAsync(x => x.Id == product.Id);
        dbContext.Products.Remove(product);

        // Act
        var act = () => dbContext.SaveChangesAsync();

        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(act);
    }

    private static Client CreateClient(string externalId) => new()
    {
        ExternalId = externalId,
        Name = "Integration Client",
        Age = 33,
        Gender = "Male",
        Location = "Saint Petersburg",
        PreviousPurchases = 4,
        FrequencyOfPurchases = "Weekly"
    };

    private static Product CreateProduct() => new()
    {
        Name = "Jacket",
        Category = "Outerwear",
        BasePrice = 40m
    };

    private static Deal CreateDeal(Client client) => new()
    {
        Client = client,
        PurchaseAmount = 0m,
        PurchaseDateUtc = DateTime.UtcNow,
        Season = "Autumn",
        PaymentMethod = "Card",
        DiscountApplied = false,
        PromoCodeUsed = false,
        ReviewRating = 4.2m
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
