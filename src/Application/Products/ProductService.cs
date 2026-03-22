using Crm.Application.Common.Exceptions;
using Crm.Application.Products.DTOs;
using Crm.Domain.Entities;
using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Crm.Application.Products;

/// <summary>
/// Сервис управления товарами.
/// </summary>
public sealed class ProductService(
    CrmDbContext dbContext,
    ILogger<ProductService> logger) : IProductService
{
    /// <inheritdoc />
    public async Task<Guid> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateCreateRequest(request);

        var normalizedName = request.Name.Trim();
        var normalizedCategory = request.Category.Trim();
        var normalizedColor = NormalizeOptional(request.Color);
        var normalizedSize = NormalizeOptional(request.Size);

        var duplicateExists = await dbContext.Set<Product>()
            .AnyAsync(
                x => x.Name == normalizedName
                    && x.Category == normalizedCategory
                    && x.Color == normalizedColor
                    && x.Size == normalizedSize,
                cancellationToken);

        if (duplicateExists)
        {
            logger.LogWarning(
                "Попытка создать дублирующий товар. Name: {Name}, Category: {Category}, Color: {Color}, Size: {Size}.",
                normalizedName,
                normalizedCategory,
                normalizedColor,
                normalizedSize);

            throw new ConflictException(
                "Товар с такими параметрами уже существует.");
        }

        var product = new Product
        {
            Name = normalizedName,
            Category = normalizedCategory,
            Color = normalizedColor,
            Size = normalizedSize,
            BasePrice = request.BasePrice,
            IsActive = true
        };

        dbContext.Set<Product>().Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Создан товар. ProductId: {ProductId}, Name: {Name}, Category: {Category}, BasePrice: {BasePrice}.",
            product.Id,
            product.Name,
            product.Category,
            product.BasePrice);

        return product.Id;
    }

    /// <inheritdoc />
    public async Task<ProductResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Set<Product>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException($"Товар '{id}' не найден.");
        }

        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category,
            Color = product.Color,
            Size = product.Size,
            BasePrice = product.BasePrice,
            IsActive = product.IsActive
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ProductListItemResponse>> GetAllAsync(
        GetProductsRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.MinPrice.HasValue && request.MinPrice < 0)
        {
            throw new ValidationException("Минимальная цена не может быть отрицательной.");
        }

        if (request.MaxPrice.HasValue && request.MaxPrice < 0)
        {
            throw new ValidationException("Максимальная цена не может быть отрицательной.");
        }

        if (request.MinPrice.HasValue && request.MaxPrice.HasValue && request.MinPrice > request.MaxPrice)
        {
            throw new ValidationException("Минимальная цена не может быть больше максимальной.");
        }

        IQueryable<Product> query = dbContext.Set<Product>()
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                x.Name.Contains(search) ||
                x.Category.Contains(search) ||
                (x.Color != null && x.Color.Contains(search)) ||
                (x.Size != null && x.Size.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            var category = request.Category.Trim();

            query = query.Where(x => x.Category == category);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(x => x.BasePrice >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(x => x.BasePrice <= request.MaxPrice.Value);
        }

        var products = await query
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Name)
            .Select(x => new ProductListItemResponse
            {
                Id = x.Id,
                Name = x.Name,
                Category = x.Category,
                Color = x.Color,
                Size = x.Size,
                BasePrice = x.BasePrice,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        return products;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateUpdateRequest(request);

        var product = await dbContext.Set<Product>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException($"Товар '{id}' не найден.");
        }

        var normalizedName = request.Name.Trim();
        var normalizedCategory = request.Category.Trim();
        var normalizedColor = NormalizeOptional(request.Color);
        var normalizedSize = NormalizeOptional(request.Size);

        var duplicateExists = await dbContext.Set<Product>()
            .AnyAsync(
                x => x.Id != id
                    && x.Name == normalizedName
                    && x.Category == normalizedCategory
                    && x.Color == normalizedColor
                    && x.Size == normalizedSize,
                cancellationToken);

        if (duplicateExists)
        {
            logger.LogWarning(
                "Попытка обновить товар до дублирующего состояния. ProductId: {ProductId}, Name: {Name}, Category: {Category}, Color: {Color}, Size: {Size}.",
                id,
                normalizedName,
                normalizedCategory,
                normalizedColor,
                normalizedSize);

            throw new ConflictException("Товар с такими параметрами уже существует.");
        }

        var oldName = product.Name;
        var oldCategory = product.Category;
        var oldBasePrice = product.BasePrice;
        var oldColor = product.Color;
        var oldSize = product.Size;

        product.Name = normalizedName;
        product.Category = normalizedCategory;
        product.Color = normalizedColor;
        product.Size = normalizedSize;
        product.BasePrice = request.BasePrice;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Обновлен товар. ProductId: {ProductId}, OldName: {OldName}, NewName: {NewName}, OldCategory: {OldCategory}, NewCategory: {NewCategory}, OldColor: {OldColor}, NewColor: {NewColor}, OldSize: {OldSize}, NewSize: {NewSize}, OldBasePrice: {OldBasePrice}, NewBasePrice: {NewBasePrice}.",
            product.Id,
            oldName,
            product.Name,
            oldCategory,
            product.Category,
            oldColor,
            product.Color,
            oldSize,
            product.Size,
            oldBasePrice,
            product.BasePrice);
    }

    /// <inheritdoc />
    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await dbContext.Set<Product>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException($"Товар '{id}' не найден.");
        }

        if (!product.IsActive)
        {
            return;
        }

        product.IsActive = false;
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Товар деактивирован. ProductId: {ProductId}, Name: {Name}.",
            product.Id,
            product.Name);
    }

    /// <inheritdoc />
    public async Task ActivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await dbContext.Set<Product>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException($"Товар '{id}' не найден.");
        }

        if (product.IsActive)
        {
            return;
        }

        product.IsActive = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Товар активирован. ProductId: {ProductId}, Name: {Name}.",
            product.Id,
            product.Name);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await dbContext.Set<Product>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException($"Товар '{id}' не найден.");
        }

        var isUsedInDeals = await dbContext.Set<DealItem>()
            .AnyAsync(x => x.ProductId == id, cancellationToken);

        var isUsedInRecommendations = await dbContext.Set<Recommendation>()
            .AnyAsync(x => x.ProductId == id, cancellationToken);

        if (isUsedInDeals || isUsedInRecommendations)
        {
            logger.LogWarning(
                "Запрещено удаление товара. ProductId: {ProductId}, IsUsedInDeals: {IsUsedInDeals}, IsUsedInRecommendations: {IsUsedInRecommendations}.",
                id,
                isUsedInDeals,
                isUsedInRecommendations);

            throw new ConflictException(
                $"Невозможно удалить товар '{id}', так как он используется в связанных данных.");
        }

        dbContext.Set<Product>().Remove(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Удален товар. ProductId: {ProductId}, Name: {Name}, Category: {Category}.",
            product.Id,
            product.Name,
            product.Category);
    }

    /// <summary>
    /// Проверяет корректность данных для создания товара.
    /// </summary>
    private static void ValidateCreateRequest(CreateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException("Наименование товара обязательно.");
        }

        if (string.IsNullOrWhiteSpace(request.Category))
        {
            throw new ValidationException("Категория товара обязательна.");
        }

        if (request.BasePrice < 0)
        {
            throw new ValidationException("Базовая цена товара не может быть отрицательной.");
        }
    }

    /// <summary>
    /// Проверяет корректность данных для обновления товара.
    /// </summary>
    private static void ValidateUpdateRequest(UpdateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException("Наименование товара обязательно.");
        }

        if (string.IsNullOrWhiteSpace(request.Category))
        {
            throw new ValidationException("Категория товара обязательна.");
        }

        if (request.BasePrice < 0)
        {
            throw new ValidationException("Базовая цена товара не может быть отрицательной.");
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