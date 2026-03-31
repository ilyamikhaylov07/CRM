namespace Crm.Application.ML.Forecasting;

/// <summary>
/// Входные признаки для модели прогнозирования продаж.
/// </summary>
public sealed class ForecastPredictionInput
{
    /// <summary>
    /// Возраст клиента.
    /// </summary>
    public float Age { get; init; }

    /// <summary>
    /// Количество предыдущих покупок.
    /// </summary>
    public float PreviousPurchases { get; init; }

    /// <summary>
    /// Общее количество сделок клиента.
    /// </summary>
    public float HistoricDealCount { get; init; }

    /// <summary>
    /// Средняя сумма покупки.
    /// </summary>
    public float AveragePurchaseAmount { get; init; }

    /// <summary>
    /// Общая сумма покупок.
    /// </summary>
    public float TotalPurchaseAmount { get; init; }

    /// <summary>
    /// Сумма последней покупки.
    /// </summary>
    public float LastPurchaseAmount { get; init; }

    /// <summary>
    /// Доля использования скидок.
    /// </summary>
    public float DiscountUsageRate { get; init; }

    /// <summary>
    /// Доля использования промокодов.
    /// </summary>
    public float PromoUsageRate { get; init; }

    /// <summary>
    /// Средний рейтинг отзывов.
    /// </summary>
    public float AverageReviewRating { get; init; }

    /// <summary>
    /// Количество активностей за последние 30 дней.
    /// </summary>
    public float ActivityCountLast30Days { get; init; }

    /// <summary>
    /// Количество активностей за последние 90 дней.
    /// </summary>
    public float ActivityCountLast90Days { get; init; }

    /// <summary>
    /// Количество открытых задач.
    /// </summary>
    public float OpenTaskCount { get; init; }

    /// <summary>
    /// Количество завершённых задач.
    /// </summary>
    public float CompletedTaskCount { get; init; }

    /// <summary>
    /// Количество дней с последней покупки.
    /// </summary>
    public float DaysSinceLastPurchase { get; init; }

    /// <summary>
    /// Количество дней с последней активности.
    /// </summary>
    public float DaysSinceLastActivity { get; init; }

    /// <summary>
    /// Среднее количество товаров в одной сделке.
    /// </summary>
    public float AvgItemsPerDeal { get; init; }

    /// <summary>
    /// Среднее количество уникальных товаров в сделке.
    /// </summary>
    public float AvgDistinctProductsPerDeal { get; init; }

    /// <summary>
    /// Доля предпочтительной категории товаров.
    /// </summary>
    public float PreferredCategoryShare { get; init; }

    /// <summary>
    /// Признак мужского пола.
    /// </summary>
    public float IsMale { get; init; }

    /// <summary>
    /// Признак женского пола.
    /// </summary>
    public float IsFemale { get; init; }

    /// <summary>
    /// Признак использования скидок.
    /// </summary>
    public float UsesDiscounts { get; init; }

    /// <summary>
    /// Признак использования промокодов.
    /// </summary>
    public float UsesPromoCodes { get; init; }
}