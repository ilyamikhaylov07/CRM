using Crm.Application.Common.Exceptions;
using Crm.Application.Deals.DTOs;
using Crm.Domain.Entities;
using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Crm.Application.Deals;

/// <summary>
/// Сервис управления сделками.
/// </summary>
public sealed class DealService(
    CrmDbContext dbContext,
    ILogger<DealService> logger) : IDealService
{
    /// <inheritdoc />
    public async Task<Guid> CreateAsync(
        CreateDealRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateCreateRequest(request);

        var client = await dbContext.Set<Client>()
            .FirstOrDefaultAsync(x => x.Id == request.ClientId, cancellationToken);

        if (client is null)
        {
            throw new NotFoundException($"Клиент '{request.ClientId}' не найден.");
        }

        User? user = null;

        if (request.UserId.HasValue)
        {
            user = await dbContext.Set<User>()
                .FirstOrDefaultAsync(x => x.Id == request.UserId.Value, cancellationToken);

            if (user is null)
            {
                throw new NotFoundException($"Пользователь '{request.UserId.Value}' не найден.");
            }
        }

        var deal = new Deal
        {
            ClientId = client.Id,
            Client = client,
            UserId = user?.Id,
            User = user,
            PurchaseAmount = request.PurchaseAmount,
            PurchaseDateUtc = request.PurchaseDateUtc,
            Season = request.Season.Trim(),
            PaymentMethod = request.PaymentMethod.Trim(),
            DiscountApplied = request.DiscountApplied,
            PromoCodeUsed = request.PromoCodeUsed,
            ReviewRating = request.ReviewRating,
            ShippingType = NormalizeOptional(request.ShippingType)
        };

        dbContext.Set<Deal>().Add(deal);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Создана сделка. DealId: {DealId}, ClientId: {ClientId}, UserId: {UserId}, PurchaseAmount: {PurchaseAmount}, PurchaseDateUtc: {PurchaseDateUtc}.",
            deal.Id,
            deal.ClientId,
            deal.UserId,
            deal.PurchaseAmount,
            deal.PurchaseDateUtc);

        return deal.Id;
    }

    /// <inheritdoc />
    public async Task<DealResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deal = await dbContext.Set<Deal>()
            .AsNoTracking()
            .Include(x => x.Client)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (deal is null)
        {
            throw new NotFoundException($"Сделка '{id}' не найдена.");
        }

        return new DealResponse
        {
            Id = deal.Id,
            ClientId = deal.ClientId,
            ClientName = deal.Client.Name,
            UserId = deal.UserId,
            UserFullName = deal.User is null
                ? null
                : $"{deal.User.FirstName} {deal.User.LastName}",
            PurchaseAmount = deal.PurchaseAmount,
            PurchaseDateUtc = deal.PurchaseDateUtc,
            Season = deal.Season,
            PaymentMethod = deal.PaymentMethod,
            DiscountApplied = deal.DiscountApplied,
            PromoCodeUsed = deal.PromoCodeUsed,
            ReviewRating = deal.ReviewRating,
            ShippingType = deal.ShippingType
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DealListItemResponse>> GetAllAsync(
        GetDealsRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateGetAllRequest(request);

        IQueryable<Deal> query = dbContext.Set<Deal>()
            .AsNoTracking()
            .Include(x => x.Client)
            .Include(x => x.User);

        if (request.ClientId.HasValue)
        {
            query = query.Where(x => x.ClientId == request.ClientId.Value);
        }

        if (request.UserId.HasValue)
        {
            query = query.Where(x => x.UserId == request.UserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.PaymentMethod))
        {
            var paymentMethod = request.PaymentMethod.Trim();
            query = query.Where(x => x.PaymentMethod == paymentMethod);
        }

        if (!string.IsNullOrWhiteSpace(request.Season))
        {
            var season = request.Season.Trim();
            query = query.Where(x => x.Season == season);
        }

        if (request.PurchaseDateFromUtc.HasValue)
        {
            query = query.Where(x => x.PurchaseDateUtc >= request.PurchaseDateFromUtc.Value);
        }

        if (request.PurchaseDateToUtc.HasValue)
        {
            query = query.Where(x => x.PurchaseDateUtc <= request.PurchaseDateToUtc.Value);
        }

        if (request.MinPurchaseAmount.HasValue)
        {
            query = query.Where(x => x.PurchaseAmount >= request.MinPurchaseAmount.Value);
        }

        if (request.MaxPurchaseAmount.HasValue)
        {
            query = query.Where(x => x.PurchaseAmount <= request.MaxPurchaseAmount.Value);
        }

        var deals = await query
            .OrderByDescending(x => x.PurchaseDateUtc)
            .ThenBy(x => x.Id)
            .Select(x => new DealListItemResponse
            {
                Id = x.Id,
                ClientId = x.ClientId,
                ClientName = x.Client.Name,
                UserId = x.UserId,
                UserFullName = x.User == null
                    ? null
                    : x.User.FirstName + " " + x.User.LastName,
                PurchaseAmount = x.PurchaseAmount,
                PurchaseDateUtc = x.PurchaseDateUtc,
                PaymentMethod = x.PaymentMethod,
                Season = x.Season
            })
            .ToListAsync(cancellationToken);

        return deals;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(
        Guid id,
        UpdateDealRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateUpdateRequest(request);

        var deal = await dbContext.Set<Deal>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (deal is null)
        {
            throw new NotFoundException($"Сделка '{id}' не найдена.");
        }

        User? user = null;

        if (request.UserId.HasValue)
        {
            user = await dbContext.Set<User>()
                .FirstOrDefaultAsync(x => x.Id == request.UserId.Value, cancellationToken);

            if (user is null)
            {
                throw new NotFoundException($"Пользователь '{request.UserId.Value}' не найден.");
            }
        }

        var oldUserId = deal.UserId;
        var oldPurchaseAmount = deal.PurchaseAmount;
        var oldPurchaseDateUtc = deal.PurchaseDateUtc;
        var oldPaymentMethod = deal.PaymentMethod;
        var oldSeason = deal.Season;
        var oldDiscountApplied = deal.DiscountApplied;
        var oldPromoCodeUsed = deal.PromoCodeUsed;
        var oldReviewRating = deal.ReviewRating;
        var oldShippingType = deal.ShippingType;

        deal.UserId = user?.Id;
        deal.User = user;
        deal.PurchaseAmount = request.PurchaseAmount;
        deal.PurchaseDateUtc = request.PurchaseDateUtc;
        deal.Season = request.Season.Trim();
        deal.PaymentMethod = request.PaymentMethod.Trim();
        deal.DiscountApplied = request.DiscountApplied;
        deal.PromoCodeUsed = request.PromoCodeUsed;
        deal.ReviewRating = request.ReviewRating;
        deal.ShippingType = NormalizeOptional(request.ShippingType);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Обновлена сделка. DealId: {DealId}, OldUserId: {OldUserId}, NewUserId: {NewUserId}, OldPurchaseAmount: {OldPurchaseAmount}, NewPurchaseAmount: {NewPurchaseAmount}, OldPurchaseDateUtc: {OldPurchaseDateUtc}, NewPurchaseDateUtc: {NewPurchaseDateUtc}, OldPaymentMethod: {OldPaymentMethod}, NewPaymentMethod: {NewPaymentMethod}, OldSeason: {OldSeason}, NewSeason: {NewSeason}, OldDiscountApplied: {OldDiscountApplied}, NewDiscountApplied: {NewDiscountApplied}, OldPromoCodeUsed: {OldPromoCodeUsed}, NewPromoCodeUsed: {NewPromoCodeUsed}, OldReviewRating: {OldReviewRating}, NewReviewRating: {NewReviewRating}, OldShippingType: {OldShippingType}, NewShippingType: {NewShippingType}.",
            deal.Id,
            oldUserId,
            deal.UserId,
            oldPurchaseAmount,
            deal.PurchaseAmount,
            oldPurchaseDateUtc,
            deal.PurchaseDateUtc,
            oldPaymentMethod,
            deal.PaymentMethod,
            oldSeason,
            deal.Season,
            oldDiscountApplied,
            deal.DiscountApplied,
            oldPromoCodeUsed,
            deal.PromoCodeUsed,
            oldReviewRating,
            deal.ReviewRating,
            oldShippingType,
            deal.ShippingType);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deal = await dbContext.Set<Deal>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (deal is null)
        {
            throw new NotFoundException($"Сделка '{id}' не найдена.");
        }

        var hasItems = await dbContext.Set<DealItem>()
            .AnyAsync(x => x.DealId == id, cancellationToken);

        var hasActivities = await dbContext.Set<Activity>()
            .AnyAsync(x => x.DealId == id, cancellationToken);

        var hasTasks = await dbContext.Set<TaskItem>()
            .AnyAsync(x => x.DealId == id, cancellationToken);

        if (hasItems || hasActivities || hasTasks)
        {
            logger.LogWarning(
                "Запрещено удаление сделки. DealId: {DealId}, HasItems: {HasItems}, HasActivities: {HasActivities}, HasTasks: {HasTasks}.",
                id,
                hasItems,
                hasActivities,
                hasTasks);

            throw new ConflictException(
                $"Невозможно удалить сделку '{id}', так как с ней связаны позиции, активности или задачи.");
        }

        dbContext.Set<Deal>().Remove(deal);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Удалена сделка. DealId: {DealId}, ClientId: {ClientId}, UserId: {UserId}, PurchaseAmount: {PurchaseAmount}.",
            deal.Id,
            deal.ClientId,
            deal.UserId,
            deal.PurchaseAmount);
    }

    /// <summary>
    /// Проверяет корректность данных для создания сделки.
    /// </summary>
    private static void ValidateCreateRequest(CreateDealRequest request)
    {
        if (request.ClientId == Guid.Empty)
        {
            throw new ValidationException("Идентификатор клиента обязателен.");
        }

        if (request.UserId.HasValue && request.UserId.Value == Guid.Empty)
        {
            throw new ValidationException("Идентификатор пользователя задан некорректно.");
        }

        if (request.PurchaseAmount < 0)
        {
            throw new ValidationException("Сумма покупки не может быть отрицательной.");
        }

        if (request.PurchaseDateUtc == default)
        {
            throw new ValidationException("Дата покупки обязательна.");
        }

        if (string.IsNullOrWhiteSpace(request.Season))
        {
            throw new ValidationException("Сезон обязателен.");
        }

        if (string.IsNullOrWhiteSpace(request.PaymentMethod))
        {
            throw new ValidationException("Способ оплаты обязателен.");
        }

        if (request.ReviewRating < 0 || request.ReviewRating > 5)
        {
            throw new ValidationException("Оценка отзыва должна быть в диапазоне от 0 до 5.");
        }
    }

    /// <summary>
    /// Проверяет корректность данных для обновления сделки.
    /// </summary>
    private static void ValidateUpdateRequest(UpdateDealRequest request)
    {
        if (request.UserId.HasValue && request.UserId.Value == Guid.Empty)
        {
            throw new ValidationException("Идентификатор пользователя задан некорректно.");
        }

        if (request.PurchaseAmount < 0)
        {
            throw new ValidationException("Сумма покупки не может быть отрицательной.");
        }

        if (request.PurchaseDateUtc == default)
        {
            throw new ValidationException("Дата покупки обязательна.");
        }

        if (string.IsNullOrWhiteSpace(request.Season))
        {
            throw new ValidationException("Сезон обязателен.");
        }

        if (string.IsNullOrWhiteSpace(request.PaymentMethod))
        {
            throw new ValidationException("Способ оплаты обязателен.");
        }

        if (request.ReviewRating < 0 || request.ReviewRating > 5)
        {
            throw new ValidationException("Оценка отзыва должна быть в диапазоне от 0 до 5.");
        }
    }

    /// <summary>
    /// Проверяет корректность параметров запроса списка сделок.
    /// </summary>
    private static void ValidateGetAllRequest(GetDealsRequest request)
    {
        if (request.ClientId.HasValue && request.ClientId.Value == Guid.Empty)
        {
            throw new ValidationException("Идентификатор клиента задан некорректно.");
        }

        if (request.UserId.HasValue && request.UserId.Value == Guid.Empty)
        {
            throw new ValidationException("Идентификатор пользователя задан некорректно.");
        }

        if (request.MinPurchaseAmount.HasValue && request.MinPurchaseAmount.Value < 0)
        {
            throw new ValidationException("Минимальная сумма покупки не может быть отрицательной.");
        }

        if (request.MaxPurchaseAmount.HasValue && request.MaxPurchaseAmount.Value < 0)
        {
            throw new ValidationException("Максимальная сумма покупки не может быть отрицательной.");
        }

        if (request.MinPurchaseAmount.HasValue
            && request.MaxPurchaseAmount.HasValue
            && request.MinPurchaseAmount.Value > request.MaxPurchaseAmount.Value)
        {
            throw new ValidationException("Минимальная сумма покупки не может быть больше максимальной.");
        }

        if (request.PurchaseDateFromUtc.HasValue
            && request.PurchaseDateToUtc.HasValue
            && request.PurchaseDateFromUtc.Value > request.PurchaseDateToUtc.Value)
        {
            throw new ValidationException("Начальная дата периода не может быть больше конечной.");
        }
    }

    /// <summary>
    /// Нормализует необязательное строковое значение.
    /// </summary>
    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}