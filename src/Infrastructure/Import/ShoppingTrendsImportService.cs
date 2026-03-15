using Crm.Domain.Entities;
using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Crm.Infrastructure.Import;

public sealed class ShoppingTrendsImportService(CrmDbContext dbContext)
{
    private readonly CrmDbContext _dbContext = dbContext;

    public async Task ImportAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var rows = ShoppingTrendsRuParser.Parse(filePath);

        var existingClients = await _dbContext.Clients
            .ToDictionaryAsync(x => x.ExternalId, cancellationToken);

        var existingProducts = await _dbContext.Products
            .ToDictionaryAsync(
                x => BuildProductKey(x.Name, x.Category, x.Color, x.Size),
                cancellationToken);

        var deals = new List<Deal>();
        var dealItems = new List<DealItem>();

        foreach (var row in rows)
        {
            if (!existingClients.TryGetValue(row.CustomerId, out var client))
            {
                client = new Client
                {
                    ExternalId = row.CustomerId,
                    Name = $"Клиент {row.CustomerId}",
                    Age = row.Age,
                    Gender = NormalizeGender(row.Gender),
                    Location = row.Location,
                    PreviousPurchases = row.PreviousPurchases,
                    FrequencyOfPurchases = NormalizeFrequency(row.FrequencyOfPurchases)
                };

                existingClients[row.CustomerId] = client;
                _dbContext.Clients.Add(client);
            }

            var productKey = BuildProductKey(
                row.ItemPurchased,
                row.Category,
                row.Color,
                row.Size);

            if (!existingProducts.TryGetValue(productKey, out var product))
            {
                product = new Product
                {
                    Name = row.ItemPurchased,
                    Category = row.Category,
                    Color = EmptyToNull(row.Color),
                    Size = EmptyToNull(row.Size),
                    BasePrice = row.PurchaseAmountUsd,
                    IsActive = true
                };

                existingProducts[productKey] = product;
                _dbContext.Products.Add(product);
            }

            var deal = new Deal
            {
                Client = client,
                PurchaseAmount = row.PurchaseAmountUsd,
                PurchaseDateUtc = GeneratePurchaseDate(row),
                Season = NormalizeSeason(row.Season),
                PaymentMethod = NormalizePaymentMethod(row.PaymentMethod),
                DiscountApplied = ToBool(row.DiscountApplied),
                PromoCodeUsed = ToBool(row.PromoCodeUsed),
                ReviewRating = row.ReviewRating,
                ShippingType = NormalizeShippingType(row.ShippingType)
            };

            deals.Add(deal);

            var dealItem = new DealItem
            {
                Deal = deal,
                Product = product,
                Quantity = 1,
                UnitPrice = row.PurchaseAmountUsd,
                TotalPrice = row.PurchaseAmountUsd
            };

            dealItems.Add(dealItem);
        }

        _dbContext.Deals.AddRange(deals);
        _dbContext.DealItems.AddRange(dealItems);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string BuildProductKey(string name, string category, string? color, string? size)
    {
        return $"{name.Trim().ToLowerInvariant()}|{category.Trim().ToLowerInvariant()}|{color?.Trim().ToLowerInvariant()}|{size?.Trim().ToLowerInvariant()}";
    }

    private static string NormalizeGender(string value)
    {
        return value.Trim() switch
        {
            "Мужские" => "Мужской",
            "Мужской" => "Мужской",
            "Женский" => "Женский",
            _ => value.Trim()
        };
    }

    private static string NormalizeFrequency(string value)
    {
        return value.Trim();
    }

    private static string NormalizeSeason(string value)
    {
        return value.Trim();
    }

    private static string NormalizePaymentMethod(string value)
    {
        return value.Trim();
    }

    private static string NormalizeShippingType(string value)
    {
        return value.Trim();
    }

    private static bool ToBool(string value)
    {
        var normalized = value.Trim().ToLowerInvariant();

        return normalized is "да" or "yes" or "true" or "1";
    }

    private static string? EmptyToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static DateTime GeneratePurchaseDate(ShoppingTrendRuRow row)
    {
        var year = 2024;
        var month = row.Season.Trim() switch
        {
            "Зима" => 1,
            "Весна" => 4,
            "Лето" => 7,
            "Осень" => 10,
            _ => 1
        };

        var day = Math.Clamp((row.PreviousPurchases % 28) + 1, 1, 28);

        return new DateTime(year, month, day, 12, 0, 0, DateTimeKind.Utc);
    }
}
