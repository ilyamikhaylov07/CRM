using Crm.Application.Common.Exceptions;
using Crm.Application.Recommendations.DTOs;
using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Crm.Application.Recommendations;

/// <summary>
/// Сервис работы с сохранёнными рекомендациями.
/// </summary>
public sealed class RecommendationService(
    CrmDbContext dbContext,
    ILogger<RecommendationService> logger) : IRecommendationService
{
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<RecommendationListItemResponse>> GetAllAsync(
        GetRecommendationsRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.FromDateUtc.HasValue && request.ToDateUtc.HasValue && request.FromDateUtc > request.ToDateUtc)
        {
            throw new ValidationException("Начальная дата не может быть больше конечной даты.");
        }

        var query = BuildQuery()
            .AsNoTracking();

        if (request.ClientId.HasValue)
        {
            query = query.Where(x => x.ClientId == request.ClientId.Value);
        }

        if (request.ProductId.HasValue)
        {
            query = query.Where(x => x.ProductId == request.ProductId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (request.FromDateUtc.HasValue)
        {
            query = query.Where(x => x.RecommendationDateUtc >= request.FromDateUtc.Value);
        }

        if (request.ToDateUtc.HasValue)
        {
            query = query.Where(x => x.RecommendationDateUtc <= request.ToDateUtc.Value);
        }

        var items = await query
            .OrderByDescending(x => x.RecommendationDateUtc)
            .ThenByDescending(x => x.Score)
            .Select(x => new RecommendationListItemResponse
            {
                Id = x.Id,
                ClientId = x.ClientId,
                ClientFullName = x.Client.Name,
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                Category = x.Product.Category,
                Score = x.Score,
                Status = x.Status,
                RecommendationDateUtc = x.RecommendationDateUtc
            })
            .ToListAsync(cancellationToken);

        return items;
    }

    /// <inheritdoc />
    public async Task<RecommendationResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var recommendation = await BuildQuery()
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new RecommendationResponse
            {
                Id = x.Id,
                ClientId = x.ClientId,
                ClientFullName = x.Client.Name,
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                Category = x.Product.Category,
                Score = x.Score,
                Reason = x.Reason,
                Status = x.Status,
                RecommendationDateUtc = x.RecommendationDateUtc,
                CreatedAtUtc = x.CreatedAtUtc,
                UpdatedAtUtc = x.UpdatedAtUtc
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (recommendation is null)
        {
            throw new NotFoundException($"Рекомендация '{id}' не найдена.");
        }

        return recommendation;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<RecommendationListItemResponse>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken)
    {
        var clientExists = await dbContext.Clients
            .AsNoTracking()
            .AnyAsync(x => x.Id == clientId, cancellationToken);

        if (!clientExists)
        {
            throw new NotFoundException($"Клиент '{clientId}' не найден.");
        }

        return await BuildQuery()
            .AsNoTracking()
            .Where(x => x.ClientId == clientId)
            .OrderByDescending(x => x.RecommendationDateUtc)
            .ThenByDescending(x => x.Score)
            .Select(x => new RecommendationListItemResponse
            {
                Id = x.Id,
                ClientId = x.ClientId,
                ClientFullName = x.Client.Name,
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                Category = x.Product.Category,
                Score = x.Score,
                Status = x.Status,
                RecommendationDateUtc = x.RecommendationDateUtc
            })
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateStatusAsync(Guid id, ChangeRecommendationStatusRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var recommendation = await dbContext.Recommendations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (recommendation is null)
        {
            throw new NotFoundException($"Рекомендация '{id}' не найдена.");
        }

        if (recommendation.Status == request.Status)
        {
            return;
        }

        recommendation.Status = request.Status;
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Обновлен статус рекомендации. RecommendationId: {RecommendationId}, Status: {Status}.",
            recommendation.Id,
            recommendation.Status);
    }

    /// <summary>
    /// Возвращает базовый запрос рекомендаций с нужными навигационными свойствами.
    /// </summary>
    /// <returns>Запрос рекомендаций.</returns>
    private IQueryable<Domain.Entities.Recommendation> BuildQuery()
    {
        return dbContext.Recommendations
            .Include(x => x.Client)
            .Include(x => x.Product);
    }
}
