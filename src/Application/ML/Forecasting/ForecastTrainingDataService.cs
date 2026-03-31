using Crm.Domain.Entities;
using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using TaskStatus = Crm.Domain.Enums.TaskStatus;

namespace Crm.Application.ML.Forecasting;

/// <summary>
/// Сервис подготовки обучающих и прогнозных данных для модели прогнозирования продаж.
/// </summary>
public sealed class ForecastTrainingDataService(CrmDbContext dbContext) : IForecastTrainingDataService
{
    /// <summary>
    /// Формирует обучающую выборку на основе исторических данных по клиентам и сделкам.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция строк обучающей выборки.</returns>
    public async Task<IReadOnlyCollection<ForecastTrainingRow>> BuildTrainingSetAsync(CancellationToken cancellationToken)
    {
        var clients = await LoadClientsAsync(cancellationToken);
        var rows = new List<ForecastTrainingRow>();

        foreach (var client in clients)
        {
            var orderedDeals = client.Deals.OrderBy(x => x.PurchaseDateUtc).ToList();

            if (orderedDeals.Count == 1)
            {
                var singleDeal = orderedDeals[0];
                var snapshot = BuildSnapshot(client, Array.Empty<Deal>(), singleDeal.PurchaseDateUtc);

                rows.Add(new ForecastTrainingRow
                {
                    Age = snapshot.Age,
                    PreviousPurchases = snapshot.PreviousPurchases,
                    HistoricDealCount = snapshot.HistoricDealCount,
                    AveragePurchaseAmount = snapshot.AveragePurchaseAmount,
                    TotalPurchaseAmount = snapshot.TotalPurchaseAmount,
                    LastPurchaseAmount = snapshot.LastPurchaseAmount,
                    DiscountUsageRate = snapshot.DiscountUsageRate,
                    PromoUsageRate = snapshot.PromoUsageRate,
                    AverageReviewRating = snapshot.AverageReviewRating,
                    ActivityCountLast30Days = snapshot.ActivityCountLast30Days,
                    ActivityCountLast90Days = snapshot.ActivityCountLast90Days,
                    OpenTaskCount = snapshot.OpenTaskCount,
                    CompletedTaskCount = snapshot.CompletedTaskCount,
                    DaysSinceLastPurchase = snapshot.DaysSinceLastPurchase,
                    DaysSinceLastActivity = snapshot.DaysSinceLastActivity,
                    AvgItemsPerDeal = snapshot.AvgItemsPerDeal,
                    AvgDistinctProductsPerDeal = snapshot.AvgDistinctProductsPerDeal,
                    PreferredCategoryShare = snapshot.PreferredCategoryShare,
                    IsMale = snapshot.IsMale,
                    IsFemale = snapshot.IsFemale,
                    UsesDiscounts = snapshot.UsesDiscounts,
                    UsesPromoCodes = snapshot.UsesPromoCodes,
                    Label = Math.Max(0F, (float)singleDeal.PurchaseAmount)
                });

                continue;
            }

            for (var index = 1; index < orderedDeals.Count; index++)
            {
                var currentDeal = orderedDeals[index];
                var historyDeals = orderedDeals.Take(index).ToList();
                var snapshot = BuildSnapshot(client, historyDeals, currentDeal.PurchaseDateUtc);

                rows.Add(new ForecastTrainingRow
                {
                    Age = snapshot.Age,
                    PreviousPurchases = snapshot.PreviousPurchases,
                    HistoricDealCount = snapshot.HistoricDealCount,
                    AveragePurchaseAmount = snapshot.AveragePurchaseAmount,
                    TotalPurchaseAmount = snapshot.TotalPurchaseAmount,
                    LastPurchaseAmount = snapshot.LastPurchaseAmount,
                    DiscountUsageRate = snapshot.DiscountUsageRate,
                    PromoUsageRate = snapshot.PromoUsageRate,
                    AverageReviewRating = snapshot.AverageReviewRating,
                    ActivityCountLast30Days = snapshot.ActivityCountLast30Days,
                    ActivityCountLast90Days = snapshot.ActivityCountLast90Days,
                    OpenTaskCount = snapshot.OpenTaskCount,
                    CompletedTaskCount = snapshot.CompletedTaskCount,
                    DaysSinceLastPurchase = snapshot.DaysSinceLastPurchase,
                    DaysSinceLastActivity = snapshot.DaysSinceLastActivity,
                    AvgItemsPerDeal = snapshot.AvgItemsPerDeal,
                    AvgDistinctProductsPerDeal = snapshot.AvgDistinctProductsPerDeal,
                    PreferredCategoryShare = snapshot.PreferredCategoryShare,
                    IsMale = snapshot.IsMale,
                    IsFemale = snapshot.IsFemale,
                    UsesDiscounts = snapshot.UsesDiscounts,
                    UsesPromoCodes = snapshot.UsesPromoCodes,
                    Label = Math.Max(0F, (float)currentDeal.PurchaseAmount)
                });
            }
        }

        return rows;
    }

    /// <summary>
    /// Формирует набор признаков для выполнения прогноза по текущим клиентам.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция снимков признаков клиентов для прогнозирования.</returns>
    public async Task<IReadOnlyCollection<ClientForecastFeatureSnapshot>> BuildPredictionSetAsync(CancellationToken cancellationToken)
    {
        var clients = await LoadClientsAsync(cancellationToken);
        var snapshots = new List<ClientForecastFeatureSnapshot>();
        var now = DateTime.UtcNow;

        foreach (var client in clients)
        {
            var historyDeals = client.Deals.OrderBy(x => x.PurchaseDateUtc).ToList();
            if (historyDeals.Count == 0)
            {
                continue;
            }

            var snapshot = BuildSnapshot(client, historyDeals, now);
            snapshots.Add(new ClientForecastFeatureSnapshot
            {
                ClientId = client.Id,
                AveragePurchaseAmount = (decimal)snapshot.AveragePurchaseAmount,
                HistoricDealCount = historyDeals.Count,
                Input = new ForecastPredictionInput
                {
                    Age = snapshot.Age,
                    PreviousPurchases = snapshot.PreviousPurchases,
                    HistoricDealCount = snapshot.HistoricDealCount,
                    AveragePurchaseAmount = snapshot.AveragePurchaseAmount,
                    TotalPurchaseAmount = snapshot.TotalPurchaseAmount,
                    LastPurchaseAmount = snapshot.LastPurchaseAmount,
                    DiscountUsageRate = snapshot.DiscountUsageRate,
                    PromoUsageRate = snapshot.PromoUsageRate,
                    AverageReviewRating = snapshot.AverageReviewRating,
                    ActivityCountLast30Days = snapshot.ActivityCountLast30Days,
                    ActivityCountLast90Days = snapshot.ActivityCountLast90Days,
                    OpenTaskCount = snapshot.OpenTaskCount,
                    CompletedTaskCount = snapshot.CompletedTaskCount,
                    DaysSinceLastPurchase = snapshot.DaysSinceLastPurchase,
                    DaysSinceLastActivity = snapshot.DaysSinceLastActivity,
                    AvgItemsPerDeal = snapshot.AvgItemsPerDeal,
                    AvgDistinctProductsPerDeal = snapshot.AvgDistinctProductsPerDeal,
                    PreferredCategoryShare = snapshot.PreferredCategoryShare,
                    IsMale = snapshot.IsMale,
                    IsFemale = snapshot.IsFemale,
                    UsesDiscounts = snapshot.UsesDiscounts,
                    UsesPromoCodes = snapshot.UsesPromoCodes
                }
            });
        }

        return snapshots;
    }

    /// <summary>
    /// Загружает клиентов вместе со связанными данными, необходимыми для расчёта признаков.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список клиентов с загруженными сделками, активностями и задачами.</returns>
    private async Task<List<Client>> LoadClientsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Clients
            .AsNoTracking()
            .Include(x => x.Deals)
                .ThenInclude(x => x.Items)
                    .ThenInclude(x => x.Product)
            .Include(x => x.Activities)
            .Include(x => x.Tasks)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Вычисляет значения признаков клиента на указанную дату на основе доступной истории.
    /// </summary>
    /// <param name="client">Клиент, для которого рассчитываются признаки.</param>
    /// <param name="historyDeals">Исторические сделки клиента, доступные на момент расчёта.</param>
    /// <param name="asOfUtc">Дата, относительно которой рассчитываются признаки.</param>
    /// <returns>Рассчитанные значения признаков клиента.</returns>
    private static SnapshotValues BuildSnapshot(Client client, IReadOnlyCollection<Deal> historyDeals, DateTime asOfUtc)
    {
        var orderedDeals = historyDeals.OrderBy(x => x.PurchaseDateUtc).ToList();
        var lastDeal = orderedDeals.LastOrDefault();
        var activitiesBeforeDate = client.Activities.Where(x => x.ActivityDateUtc <= asOfUtc).ToList();
        var tasksBeforeDate = client.Tasks.Where(x => x.CreatedAtUtc <= asOfUtc).ToList();
        var allItems = orderedDeals.SelectMany(x => x.Items).ToList();
        var totalItemQuantity = allItems.Sum(x => x.Quantity);
        var categoryTotals = allItems
            .Where(x => x.Product is not null)
            .GroupBy(x => x.Product.Category)
            .Select(x => x.Sum(item => item.Quantity))
            .ToList();

        return new SnapshotValues
        {
            Age = client.Age,
            PreviousPurchases = client.PreviousPurchases,
            HistoricDealCount = orderedDeals.Count,
            AveragePurchaseAmount = orderedDeals.Count == 0 ? 0F : (float)orderedDeals.Average(x => x.PurchaseAmount),
            TotalPurchaseAmount = (float)orderedDeals.Sum(x => x.PurchaseAmount),
            LastPurchaseAmount = lastDeal is null ? 0F : (float)lastDeal.PurchaseAmount,
            DiscountUsageRate = orderedDeals.Count == 0 ? 0F : (float)orderedDeals.Count(x => x.DiscountApplied) / orderedDeals.Count,
            PromoUsageRate = orderedDeals.Count == 0 ? 0F : (float)orderedDeals.Count(x => x.PromoCodeUsed) / orderedDeals.Count,
            AverageReviewRating = orderedDeals.Count == 0 ? 0F : (float)orderedDeals.Average(x => x.ReviewRating),
            ActivityCountLast30Days = activitiesBeforeDate.Count(x => x.ActivityDateUtc >= asOfUtc.AddDays(-30)),
            ActivityCountLast90Days = activitiesBeforeDate.Count(x => x.ActivityDateUtc >= asOfUtc.AddDays(-90)),
            OpenTaskCount = tasksBeforeDate.Count(x => x.Status is TaskStatus.New or TaskStatus.InProgress),
            CompletedTaskCount = tasksBeforeDate.Count(x => x.Status == TaskStatus.Completed),
            DaysSinceLastPurchase = lastDeal is null ? 999F : (float)Math.Max(0, (asOfUtc - lastDeal.PurchaseDateUtc).TotalDays),
            DaysSinceLastActivity = activitiesBeforeDate.Count == 0 ? 999F : (float)Math.Max(0, (asOfUtc - activitiesBeforeDate.Max(x => x.ActivityDateUtc)).TotalDays),
            AvgItemsPerDeal = orderedDeals.Count == 0 ? 0F : (float)allItems.Sum(x => x.Quantity) / orderedDeals.Count,
            AvgDistinctProductsPerDeal = orderedDeals.Count == 0 ? 0F : (float)orderedDeals.Average(x => x.Items.Select(i => i.ProductId).Distinct().Count()),
            PreferredCategoryShare = totalItemQuantity == 0M || categoryTotals.Count == 0 ? 0F : (float)(categoryTotals.Max() / totalItemQuantity),
            IsMale = string.Equals(client.Gender, "Мужской", StringComparison.OrdinalIgnoreCase) ? 1F : 0F,
            IsFemale = string.Equals(client.Gender, "Женский", StringComparison.OrdinalIgnoreCase) ? 1F : 0F,
            UsesDiscounts = orderedDeals.Any(x => x.DiscountApplied) ? 1F : 0F,
            UsesPromoCodes = orderedDeals.Any(x => x.PromoCodeUsed) ? 1F : 0F
        };
    }

    /// <summary>
    /// Внутренний контейнер для хранения рассчитанных признаков клиента.
    /// </summary>
    private class SnapshotValues
    {
        public float Age { get; init; }
        public float PreviousPurchases { get; init; }
        public float HistoricDealCount { get; init; }
        public float AveragePurchaseAmount { get; init; }
        public float TotalPurchaseAmount { get; init; }
        public float LastPurchaseAmount { get; init; }
        public float DiscountUsageRate { get; init; }
        public float PromoUsageRate { get; init; }
        public float AverageReviewRating { get; init; }
        public float ActivityCountLast30Days { get; init; }
        public float ActivityCountLast90Days { get; init; }
        public float OpenTaskCount { get; init; }
        public float CompletedTaskCount { get; init; }
        public float DaysSinceLastPurchase { get; init; }
        public float DaysSinceLastActivity { get; init; }
        public float AvgItemsPerDeal { get; init; }
        public float AvgDistinctProductsPerDeal { get; init; }
        public float PreferredCategoryShare { get; init; }
        public float IsMale { get; init; }
        public float IsFemale { get; init; }
        public float UsesDiscounts { get; init; }
        public float UsesPromoCodes { get; init; }
    }
}
