using Crm.Application.Common.Exceptions;
using Crm.Application.DealItems.DTOs;
using Crm.Domain.Entities;
using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Crm.Application.DealItems;

/// <summary>
/// Сервис управления позициями сделок.
/// </summary>
public sealed class DealItemService(
    CrmDbContext dbContext,
    IDealAmountCalculator dealAmountCalculator,
    ILogger<DealItemService> logger) : IDealItemService
{
    /// <inheritdoc />
    public async Task<Guid> CreateAsync(
        Guid dealId,
        CreateDealItemRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateCreateRequest(dealId, request);

        var deal = await dbContext.Set<Deal>()
            .FirstOrDefaultAsync(x => x.Id == dealId, cancellationToken);

        if (deal is null)
        {
            throw new NotFoundException($"Сделка '{dealId}' не найдена.");
        }

        var product = await dbContext.Set<Product>()
            .FirstOrDefaultAsync(x => x.Id == request.ProductId, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException($"Товар '{request.ProductId}' не найден.");
        }

        if (!product.IsActive)
        {
            throw new ConflictException(
                $"Невозможно добавить в сделку неактивный товар '{request.ProductId}'.");
        }

        var totalPrice = CalculateTotalPrice(request.Quantity, request.UnitPrice);

        var item = new DealItem
        {
            DealId = deal.Id,
            Deal = deal,
            ProductId = product.Id,
            Product = product,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            TotalPrice = totalPrice
        };

        dbContext.Set<DealItem>().Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);

        await dealAmountCalculator.RecalculateAsync(dealId, cancellationToken);

        logger.LogInformation(
            "Добавлена позиция сделки. DealItemId: {DealItemId}, DealId: {DealId}, ProductId: {ProductId}, Quantity: {Quantity}, UnitPrice: {UnitPrice}, TotalPrice: {TotalPrice}.",
            item.Id,
            item.DealId,
            item.ProductId,
            item.Quantity,
            item.UnitPrice,
            item.TotalPrice);

        return item.Id;
    }

    /// <inheritdoc />
    public async Task<DealItemResponse> GetByIdAsync(
        Guid dealId,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        ValidateIdentifiers(dealId, itemId);

        var item = await dbContext.Set<DealItem>()
            .AsNoTracking()
            .Include(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == itemId && x.DealId == dealId, cancellationToken);

        if (item is null)
        {
            throw new NotFoundException(
                $"Позиция сделки '{itemId}' для сделки '{dealId}' не найдена.");
        }

        return new DealItemResponse
        {
            Id = item.Id,
            DealId = item.DealId,
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            ProductCategory = item.Product.Category,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<DealItemListItemResponse>> GetAllAsync(
        Guid dealId,
        CancellationToken cancellationToken)
    {
        if (dealId == Guid.Empty)
        {
            throw new ValidationException("Идентификатор сделки обязателен.");
        }

        var dealExists = await dbContext.Set<Deal>()
            .AnyAsync(x => x.Id == dealId, cancellationToken);

        if (!dealExists)
        {
            throw new NotFoundException($"Сделка '{dealId}' не найдена.");
        }

        var items = await dbContext.Set<DealItem>()
            .AsNoTracking()
            .Include(x => x.Product)
            .Where(x => x.DealId == dealId)
            .OrderBy(x => x.Id)
            .Select(x => new DealItemListItemResponse
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                TotalPrice = x.TotalPrice
            })
            .ToListAsync(cancellationToken);

        return items;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(
        Guid dealId,
        Guid itemId,
        UpdateDealItemRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateUpdateRequest(dealId, itemId, request);

        var item = await dbContext.Set<DealItem>()
            .FirstOrDefaultAsync(x => x.Id == itemId && x.DealId == dealId, cancellationToken);

        if (item is null)
        {
            throw new NotFoundException(
                $"Позиция сделки '{itemId}' для сделки '{dealId}' не найдена.");
        }

        var product = await dbContext.Set<Product>()
            .FirstOrDefaultAsync(x => x.Id == request.ProductId, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException($"Товар '{request.ProductId}' не найден.");
        }

        if (!product.IsActive)
        {
            throw new ConflictException(
                $"Невозможно использовать в позиции неактивный товар '{request.ProductId}'.");
        }

        var oldProductId = item.ProductId;
        var oldQuantity = item.Quantity;
        var oldUnitPrice = item.UnitPrice;
        var oldTotalPrice = item.TotalPrice;

        item.ProductId = product.Id;
        item.Product = product;
        item.Quantity = request.Quantity;
        item.UnitPrice = request.UnitPrice;
        item.TotalPrice = CalculateTotalPrice(request.Quantity, request.UnitPrice);

        await dbContext.SaveChangesAsync(cancellationToken);

        await dealAmountCalculator.RecalculateAsync(dealId, cancellationToken);

        logger.LogInformation(
            "Обновлена позиция сделки. DealItemId: {DealItemId}, DealId: {DealId}, OldProductId: {OldProductId}, NewProductId: {NewProductId}, OldQuantity: {OldQuantity}, NewQuantity: {NewQuantity}, OldUnitPrice: {OldUnitPrice}, NewUnitPrice: {NewUnitPrice}, OldTotalPrice: {OldTotalPrice}, NewTotalPrice: {NewTotalPrice}.",
            item.Id,
            item.DealId,
            oldProductId,
            item.ProductId,
            oldQuantity,
            item.Quantity,
            oldUnitPrice,
            item.UnitPrice,
            oldTotalPrice,
            item.TotalPrice);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(
        Guid dealId,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        ValidateIdentifiers(dealId, itemId);

        var item = await dbContext.Set<DealItem>()
            .FirstOrDefaultAsync(x => x.Id == itemId && x.DealId == dealId, cancellationToken);

        if (item is null)
        {
            throw new NotFoundException(
                $"Позиция сделки '{itemId}' для сделки '{dealId}' не найдена.");
        }

        dbContext.Set<DealItem>().Remove(item);
        await dbContext.SaveChangesAsync(cancellationToken);

        await dealAmountCalculator.RecalculateAsync(dealId, cancellationToken);

        logger.LogInformation(
            "Удалена позиция сделки. DealItemId: {DealItemId}, DealId: {DealId}, ProductId: {ProductId}, TotalPrice: {TotalPrice}.",
            item.Id,
            item.DealId,
            item.ProductId,
            item.TotalPrice);
    }

    /// <summary>
    /// Проверяет корректность данных для создания позиции сделки.
    /// </summary>
    private static void ValidateCreateRequest(Guid dealId, CreateDealItemRequest request)
    {
        if (dealId == Guid.Empty)
        {
            throw new ValidationException("Идентификатор сделки обязателен.");
        }

        if (request.ProductId == Guid.Empty)
        {
            throw new ValidationException("Идентификатор товара обязателен.");
        }

        if (request.Quantity <= 0)
        {
            throw new ValidationException("Количество товара должно быть больше нуля.");
        }

        if (request.UnitPrice < 0)
        {
            throw new ValidationException("Цена за единицу товара не может быть отрицательной.");
        }
    }

    /// <summary>
    /// Проверяет корректность данных для обновления позиции сделки.
    /// </summary>
    private static void ValidateUpdateRequest(Guid dealId, Guid itemId, UpdateDealItemRequest request)
    {
        ValidateIdentifiers(dealId, itemId);

        if (request.ProductId == Guid.Empty)
        {
            throw new ValidationException("Идентификатор товара обязателен.");
        }

        if (request.Quantity <= 0)
        {
            throw new ValidationException("Количество товара должно быть больше нуля.");
        }

        if (request.UnitPrice < 0)
        {
            throw new ValidationException("Цена за единицу товара не может быть отрицательной.");
        }
    }

    /// <summary>
    /// Проверяет корректность идентификаторов сделки и позиции.
    /// </summary>
    private static void ValidateIdentifiers(Guid dealId, Guid itemId)
    {
        if (dealId == Guid.Empty)
        {
            throw new ValidationException("Идентификатор сделки обязателен.");
        }

        if (itemId == Guid.Empty)
        {
            throw new ValidationException("Идентификатор позиции сделки обязателен.");
        }
    }

    /// <summary>
    /// Вычисляет итоговую стоимость позиции сделки.
    /// </summary>
    private static decimal CalculateTotalPrice(decimal quantity, decimal unitPrice)
    {
        return decimal.Round(quantity * unitPrice, 2, MidpointRounding.AwayFromZero);
    }
}