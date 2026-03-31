using Crm.Application.Common.Exceptions;
using Crm.Application.Helpers.RecommendationDataSeed.DTOs;
using Crm.Domain.Entities;
using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Crm.Application.Helpers.RecommendationDataSeed;

/// <summary>
/// Helper-сервис для одноразового наполнения истории покупок под обучение рекомендательной модели.
/// </summary>
public sealed class RecommendationDataSeedHelper(
    CrmDbContext dbContext,
    ILogger<RecommendationDataSeedHelper> logger) : IRecommendationDataSeedHelper
{
    private const string SeedMarker = "[helper-recommendation-seed]";
    private static readonly string[] PaymentMethods = ["Кредитная карта", "Дебетовая карта", "Наличные", "PayPal", "Venmo", "Банковский перевод"];
    private static readonly string[] Seasons = ["Зима", "Весна", "Лето", "Осень"];

    /// <summary>
    /// Генерирует дополнительные сделки и позиции сделок на основе существующих клиентов и товаров.
    /// </summary>
    /// <param name="request">Параметры генерации тестовых данных.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат генерации данных.</returns>
    public async Task<SeedRecommendationDataResponse> SeedAsync(
        SeedRecommendationDataRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateRequest(request);

        var clients = await dbContext.Clients
            .AsNoTracking()
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        var products = await dbContext.Products
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

        if (clients.Count < 3)
        {
            throw new ValidationException("Для helper-генерации нужно минимум 3 клиента в базе.");
        }

        if (products.Count < 6)
        {
            throw new ValidationException("Для helper-генерации нужно минимум 6 активных товаров в базе.");
        }

        var categories = products
            .GroupBy(x => x.Category)
            .Where(x => !string.IsNullOrWhiteSpace(x.Key) && x.Count() >= 2)
            .OrderByDescending(x => x.Count())
            .Select(x => x.ToList())
            .ToList();

        if (categories.Count < 2)
        {
            throw new ValidationException("Для осмысленной генерации нужно минимум 2 категории с хотя бы 2 товарами в каждой.");
        }

        using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        if (request.ReplacePreviousSeedData)
        {
            await CleanupPreviousSeedDataAsync(cancellationToken);
        }

        var coveredClientCount = Math.Max(1, (int)Math.Round(clients.Count * request.ClientCoverageRatio, MidpointRounding.AwayFromZero));
        var selectedClients = clients.Take(coveredClientCount).ToList();
        var deals = new List<Deal>();
        var dealItems = new List<DealItem>();

        for (var clientIndex = 0; clientIndex < selectedClients.Count; clientIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var client = selectedClients[clientIndex];
            var primaryCategoryProducts = categories[clientIndex % categories.Count];
            var secondaryCategoryProducts = categories[(clientIndex + 1) % categories.Count];

            for (var dealIndex = 0; dealIndex < request.DealsPerClient; dealIndex++)
            {
                var itemsPerDeal = Math.Min(
                    request.MaxItemsPerDeal,
                    Math.Max(request.MinItemsPerDeal, 2 + ((clientIndex + dealIndex) % 2)));
                var selectedProducts = PickProductsForDeal(
                    primaryCategoryProducts,
                    secondaryCategoryProducts,
                    products,
                    itemsPerDeal,
                    clientIndex,
                    dealIndex);

                var deal = new Deal
                {
                    ClientId = client.Id,
                    Client = null!,
                    UserId = null,
                    PurchaseDateUtc = BuildPurchaseDate(clientIndex, dealIndex),
                    PurchaseAmount = 0M,
                    Season = Seasons[(clientIndex + dealIndex) % Seasons.Length],
                    PaymentMethod = PaymentMethods[(clientIndex + dealIndex) % PaymentMethods.Length],
                    DiscountApplied = ((clientIndex + dealIndex) % 3) == 0,
                    PromoCodeUsed = ((clientIndex + dealIndex) % 4) == 0,
                    ReviewRating = 3 + ((clientIndex + dealIndex) % 3),
                    ShippingType = SeedMarker
                };

                decimal totalAmount = 0M;

                for (var itemIndex = 0; itemIndex < selectedProducts.Count; itemIndex++)
                {
                    var product = selectedProducts[itemIndex];
                    var quantity = 1 + ((clientIndex + dealIndex + itemIndex) % 2);
                    var unitPrice = product.BasePrice > 0M
                        ? product.BasePrice
                        : 10M + ((clientIndex + itemIndex) % 10) * 5M;
                    var totalPrice = unitPrice * quantity;
                    totalAmount += totalPrice;

                    dealItems.Add(new DealItem
                    {
                        Deal = deal,
                        ProductId = product.Id,
                        Product = null!,
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        TotalPrice = totalPrice
                    });
                }

                deal.PurchaseAmount = totalAmount;
                deals.Add(deal);
            }
        }

        dbContext.Deals.AddRange(deals);
        dbContext.DealItems.AddRange(dealItems);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation(
            "Сгенерирована helper-история покупок для рекомендаций. ClientsAffected: {ClientsAffected}, DealsCreated: {DealsCreated}, DealItemsCreated: {DealItemsCreated}, CategoriesUsed: {CategoriesUsed}.",
            selectedClients.Count,
            deals.Count,
            dealItems.Count,
            categories.Count);

        return new SeedRecommendationDataResponse
        {
            ClientsAffected = selectedClients.Count,
            DealsCreated = deals.Count,
            DealItemsCreated = dealItems.Count,
            CategoriesUsed = categories.Count,
            Message = "Helper-история покупок успешно сгенерирована на основе существующих клиентов и товаров."
        };
    }

    /// <summary>
    /// Проверяет корректность параметров helper-генерации.
    /// </summary>
    /// <param name="request">Параметры генерации.</param>
    private static void ValidateRequest(SeedRecommendationDataRequest request)
    {
        if (request.DealsPerClient <= 0)
        {
            throw new ValidationException("Количество сделок на клиента должно быть больше нуля.");
        }

        if (request.MinItemsPerDeal <= 0)
        {
            throw new ValidationException("Минимальное количество товаров в сделке должно быть больше нуля.");
        }

        if (request.MaxItemsPerDeal < request.MinItemsPerDeal)
        {
            throw new ValidationException("Максимальное количество товаров в сделке не может быть меньше минимального.");
        }

        if (request.ClientCoverageRatio <= 0M || request.ClientCoverageRatio > 1.0M)
        {
            throw new ValidationException("Доля охвата клиентов должна быть в диапазоне от 0 до 1.");
        }
    }

    /// <summary>
    /// Удаляет ранее сгенерированные helper-сделки и их позиции.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    private async Task CleanupPreviousSeedDataAsync(CancellationToken cancellationToken)
    {
        var helperDealIds = await dbContext.Deals
            .AsNoTracking()
            .Where(x => x.ShippingType == SeedMarker)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (helperDealIds.Count == 0)
        {
            return;
        }

        await dbContext.DealItems
            .Where(x => helperDealIds.Contains(x.DealId))
            .ExecuteDeleteAsync(cancellationToken);

        await dbContext.Deals
            .Where(x => helperDealIds.Contains(x.Id))
            .ExecuteDeleteAsync(cancellationToken);
    }

    /// <summary>
    /// Строит дату покупки для синтетической сделки.
    /// </summary>
    /// <param name="clientIndex">Порядковый номер клиента.</param>
    /// <param name="dealIndex">Порядковый номер сделки клиента.</param>
    /// <returns>Дата покупки в UTC.</returns>
    private static DateTime BuildPurchaseDate(int clientIndex, int dealIndex)
    {
        return DateTime.UtcNow.Date
            .AddDays(-90 + (clientIndex * 3) + (dealIndex * 12))
            .AddHours(10 + (dealIndex % 8));
    }

    /// <summary>
    /// Подбирает осмысленный набор товаров для одной сделки.
    /// </summary>
    /// <param name="primaryCategoryProducts">Товары основной категории клиента.</param>
    /// <param name="secondaryCategoryProducts">Товары вторичной категории клиента.</param>
    /// <param name="allProducts">Все доступные товары.</param>
    /// <param name="itemsPerDeal">Количество товаров в сделке.</param>
    /// <param name="clientIndex">Порядковый номер клиента.</param>
    /// <param name="dealIndex">Порядковый номер сделки клиента.</param>
    /// <returns>Список выбранных товаров.</returns>
    private static List<Product> PickProductsForDeal(
        List<Product> primaryCategoryProducts,
        List<Product> secondaryCategoryProducts,
        List<Product> allProducts,
        int itemsPerDeal,
        int clientIndex,
        int dealIndex)
    {
        var selected = new List<Product>();
        var primaryTake = Math.Max(1, itemsPerDeal - 1);

        for (var index = 0; index < primaryTake && index < primaryCategoryProducts.Count; index++)
        {
            selected.Add(primaryCategoryProducts[(clientIndex + dealIndex + index) % primaryCategoryProducts.Count]);
        }

        if (selected.Count < itemsPerDeal && secondaryCategoryProducts.Count > 0)
        {
            selected.Add(secondaryCategoryProducts[(clientIndex + dealIndex) % secondaryCategoryProducts.Count]);
        }

        while (selected.Count < itemsPerDeal)
        {
            var fallback = allProducts[(clientIndex + dealIndex + selected.Count) % allProducts.Count];
            if (selected.All(x => x.Id != fallback.Id))
            {
                selected.Add(fallback);
            }
            else
            {
                break;
            }
        }

        return selected
            .GroupBy(x => x.Id)
            .Select(x => x.First())
            .Take(itemsPerDeal)
            .ToList();
    }
}
