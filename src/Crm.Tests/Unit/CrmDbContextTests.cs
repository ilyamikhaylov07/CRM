using Crm.Domain.Entities;
using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Crm.Tests.Unit;

public sealed class CrmDbContextTests
{
    [Fact]
    public async Task SaveChangesAsync_NewEntity_AssignsIdAndCreatedAt()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var client = new Client
        {
            ExternalId = "client-1",
            Name = "Test Client",
            Age = 30,
            Gender = "Female",
            Location = "Moscow",
            PreviousPurchases = 2,
            FrequencyOfPurchases = "Monthly"
        };

        // Act
        dbContext.Clients.Add(client);
        await dbContext.SaveChangesAsync();

        // Assert
        Assert.NotEqual(Guid.Empty, client.Id);
        Assert.NotEqual(default, client.CreatedAtUtc);
        Assert.Null(client.UpdatedAtUtc);
    }

    [Fact]
    public async Task SaveChangesAsync_ModifiedEntity_SetsUpdatedAt()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var product = new Product
        {
            Name = "Sneakers",
            Category = "Footwear",
            BasePrice = 120m
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        // Act
        product.BasePrice = 135m;
        await dbContext.SaveChangesAsync();

        // Assert
        Assert.NotNull(product.UpdatedAtUtc);
    }

    private static CrmDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CrmDbContext(options);
    }
}
