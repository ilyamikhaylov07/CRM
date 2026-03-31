using Crm.Application.Common.Exceptions;
using Crm.Application.Forecasts.DTOs;
using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Crm.Application.Forecasts;

/// <summary>
/// Сервис работы с сохранёнными прогнозами продаж.
/// </summary>
public sealed class ForecastService(CrmDbContext dbContext) : IForecastService
{
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ForecastListItemResponse>> GetAllAsync(
        GetForecastsRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.FromDateUtc.HasValue && request.ToDateUtc.HasValue && request.FromDateUtc > request.ToDateUtc)
        {
            throw new ValidationException("Начальная дата прогноза не может быть больше конечной даты.");
        }

        if (request.PeriodStartUtc.HasValue && request.PeriodEndUtc.HasValue && request.PeriodStartUtc > request.PeriodEndUtc)
        {
            throw new ValidationException("Начало прогнозируемого периода не может быть больше конца периода.");
        }

        var query = BuildQuery()
            .AsNoTracking();

        if (request.ClientId.HasValue)
        {
            query = query.Where(x => x.ClientId == request.ClientId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.ModelVersion))
        {
            var modelVersion = request.ModelVersion.Trim();
            query = query.Where(x => x.ModelVersion == modelVersion);
        }

        if (request.FromDateUtc.HasValue)
        {
            query = query.Where(x => x.ForecastDateUtc >= request.FromDateUtc.Value);
        }

        if (request.ToDateUtc.HasValue)
        {
            query = query.Where(x => x.ForecastDateUtc <= request.ToDateUtc.Value);
        }

        if (request.PeriodStartUtc.HasValue)
        {
            query = query.Where(x => x.PeriodStartUtc >= request.PeriodStartUtc.Value);
        }

        if (request.PeriodEndUtc.HasValue)
        {
            query = query.Where(x => x.PeriodEndUtc <= request.PeriodEndUtc.Value);
        }

        return await query
            .OrderByDescending(x => x.ForecastDateUtc)
            .ThenByDescending(x => x.PredictedAmount)
            .Select(x => new ForecastListItemResponse
            {
                Id = x.Id,
                ClientId = x.ClientId,
                ClientName = x.Client.Name,
                ForecastDateUtc = x.ForecastDateUtc,
                PeriodStartUtc = x.PeriodStartUtc,
                PeriodEndUtc = x.PeriodEndUtc,
                PredictedAmount = x.PredictedAmount,
                ConfidenceScore = x.ConfidenceScore,
                ModelVersion = x.ModelVersion
            })
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ForecastResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var forecast = await BuildQuery()
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ForecastResponse
            {
                Id = x.Id,
                ClientId = x.ClientId,
                ClientName = x.Client.Name,
                ForecastDateUtc = x.ForecastDateUtc,
                PeriodStartUtc = x.PeriodStartUtc,
                PeriodEndUtc = x.PeriodEndUtc,
                PredictedAmount = x.PredictedAmount,
                ConfidenceScore = x.ConfidenceScore,
                ModelVersion = x.ModelVersion,
                Notes = x.Notes,
                CreatedAtUtc = x.CreatedAtUtc,
                UpdatedAtUtc = x.UpdatedAtUtc
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (forecast is null)
        {
            throw new NotFoundException($"Прогноз '{id}' не найден.");
        }

        return forecast;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ForecastListItemResponse>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken)
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
            .OrderByDescending(x => x.ForecastDateUtc)
            .ThenByDescending(x => x.PredictedAmount)
            .Select(x => new ForecastListItemResponse
            {
                Id = x.Id,
                ClientId = x.ClientId,
                ClientName = x.Client.Name,
                ForecastDateUtc = x.ForecastDateUtc,
                PeriodStartUtc = x.PeriodStartUtc,
                PeriodEndUtc = x.PeriodEndUtc,
                PredictedAmount = x.PredictedAmount,
                ConfidenceScore = x.ConfidenceScore,
                ModelVersion = x.ModelVersion
            })
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Возвращает базовый запрос прогнозов с нужными навигационными свойствами.
    /// </summary>
    /// <returns>Запрос прогнозов.</returns>
    private IQueryable<Domain.Entities.SalesForecast> BuildQuery()
    {
        return dbContext.SalesForecasts
            .Include(x => x.Client);
    }
}
