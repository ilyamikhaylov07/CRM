using Crm.Application.Activities.DTOs;
using Crm.Application.Common.Exceptions;
using Crm.Domain.Entities;
using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Crm.Application.Activities;

/// <summary>
/// Сервис управления активностями.
/// </summary>
public sealed class ActivityService(
    CrmDbContext dbContext,
    ILogger<ActivityService> logger) : IActivityService
{
    /// <inheritdoc />
    public async Task<Guid> CreateAsync(
        CreateActivityRequest request,
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

        Deal? deal = null;
        if (request.DealId.HasValue)
        {
            deal = await dbContext.Set<Deal>()
                .FirstOrDefaultAsync(x => x.Id == request.DealId.Value, cancellationToken);

            if (deal is null)
            {
                throw new NotFoundException($"Сделка '{request.DealId.Value}' не найдена.");
            }

            if (deal.ClientId != request.ClientId)
            {
                throw new ConflictException(
                    $"Сделка '{request.DealId.Value}' не принадлежит клиенту '{request.ClientId}'.");
            }
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

        var activity = new Activity
        {
            Type = request.Type,
            Subject = NormalizeOptional(request.Subject),
            Description = NormalizeOptional(request.Description),
            ActivityDateUtc = request.ActivityDateUtc,
            UserId = user?.Id,
            User = user,
            ClientId = client.Id,
            Client = client,
            DealId = deal?.Id,
            Deal = deal
        };

        dbContext.Set<Activity>().Add(activity);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Создана активность. ActivityId: {ActivityId}, Type: {Type}, ClientId: {ClientId}, DealId: {DealId}, UserId: {UserId}, ActivityDateUtc: {ActivityDateUtc}.",
            activity.Id,
            activity.Type,
            activity.ClientId,
            activity.DealId,
            activity.UserId,
            activity.ActivityDateUtc);

        return activity.Id;
    }

    /// <inheritdoc />
    public async Task<ActivityResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException("Идентификатор активности обязателен.");
        }

        var activity = await dbContext.Set<Activity>()
            .AsNoTracking()
            .Include(x => x.Client)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (activity is null)
        {
            throw new NotFoundException($"Активность '{id}' не найдена.");
        }

        return new ActivityResponse
        {
            Id = activity.Id,
            Type = activity.Type,
            Subject = activity.Subject,
            Description = activity.Description,
            ActivityDateUtc = activity.ActivityDateUtc,
            UserId = activity.UserId,
            UserFullName = activity.User is null
                ? null
                : $"{activity.User.FirstName} {activity.User.LastName}",
            ClientId = activity.ClientId,
            ClientName = activity.Client.Name,
            DealId = activity.DealId
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ActivityListItemResponse>> GetAllAsync(
        GetActivitiesRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateGetAllRequest(request);

        IQueryable<Activity> query = dbContext.Set<Activity>()
            .AsNoTracking()
            .Include(x => x.Client)
            .Include(x => x.User);

        if (request.ClientId.HasValue)
        {
            query = query.Where(x => x.ClientId == request.ClientId.Value);
        }

        if (request.DealId.HasValue)
        {
            query = query.Where(x => x.DealId == request.DealId.Value);
        }

        if (request.UserId.HasValue)
        {
            query = query.Where(x => x.UserId == request.UserId.Value);
        }

        if (request.Type.HasValue)
        {
            query = query.Where(x => x.Type == request.Type.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                (x.Subject != null && x.Subject.Contains(search)) ||
                (x.Description != null && x.Description.Contains(search)));
        }

        if (request.ActivityDateFromUtc.HasValue)
        {
            query = query.Where(x => x.ActivityDateUtc >= request.ActivityDateFromUtc.Value);
        }

        if (request.ActivityDateToUtc.HasValue)
        {
            query = query.Where(x => x.ActivityDateUtc <= request.ActivityDateToUtc.Value);
        }

        var activities = await query
            .OrderByDescending(x => x.ActivityDateUtc)
            .ThenBy(x => x.Id)
            .Select(x => new ActivityListItemResponse
            {
                Id = x.Id,
                Type = x.Type,
                Subject = x.Subject,
                ActivityDateUtc = x.ActivityDateUtc,
                UserId = x.UserId,
                UserFullName = x.User == null
                    ? null
                    : x.User.FirstName + " " + x.User.LastName,
                ClientId = x.ClientId,
                ClientName = x.Client.Name,
                DealId = x.DealId
            })
            .ToListAsync(cancellationToken);

        return activities;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(
        Guid id,
        UpdateActivityRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (id == Guid.Empty)
        {
            throw new ValidationException("Идентификатор активности обязателен.");
        }

        ValidateUpdateRequest(request);

        var activity = await dbContext.Set<Activity>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (activity is null)
        {
            throw new NotFoundException($"Активность '{id}' не найдена.");
        }

        var client = await dbContext.Set<Client>()
            .FirstOrDefaultAsync(x => x.Id == request.ClientId, cancellationToken);

        if (client is null)
        {
            throw new NotFoundException($"Клиент '{request.ClientId}' не найден.");
        }

        Deal? deal = null;
        if (request.DealId.HasValue)
        {
            deal = await dbContext.Set<Deal>()
                .FirstOrDefaultAsync(x => x.Id == request.DealId.Value, cancellationToken);

            if (deal is null)
            {
                throw new NotFoundException($"Сделка '{request.DealId.Value}' не найдена.");
            }

            if (deal.ClientId != request.ClientId)
            {
                throw new ConflictException(
                    $"Сделка '{request.DealId.Value}' не принадлежит клиенту '{request.ClientId}'.");
            }
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

        var oldType = activity.Type;
        var oldSubject = activity.Subject;
        var oldActivityDateUtc = activity.ActivityDateUtc;
        var oldUserId = activity.UserId;
        var oldClientId = activity.ClientId;
        var oldDealId = activity.DealId;

        activity.Type = request.Type;
        activity.Subject = NormalizeOptional(request.Subject);
        activity.Description = NormalizeOptional(request.Description);
        activity.ActivityDateUtc = request.ActivityDateUtc;
        activity.UserId = user?.Id;
        activity.User = user;
        activity.ClientId = client.Id;
        activity.Client = client;
        activity.DealId = deal?.Id;
        activity.Deal = deal;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Обновлена активность. ActivityId: {ActivityId}, OldType: {OldType}, NewType: {NewType}, OldSubject: {OldSubject}, NewSubject: {NewSubject}, OldActivityDateUtc: {OldActivityDateUtc}, NewActivityDateUtc: {NewActivityDateUtc}, OldUserId: {OldUserId}, NewUserId: {NewUserId}, OldClientId: {OldClientId}, NewClientId: {NewClientId}, OldDealId: {OldDealId}, NewDealId: {NewDealId}.",
            activity.Id,
            oldType,
            activity.Type,
            oldSubject,
            activity.Subject,
            oldActivityDateUtc,
            activity.ActivityDateUtc,
            oldUserId,
            activity.UserId,
            oldClientId,
            activity.ClientId,
            oldDealId,
            activity.DealId);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException("Идентификатор активности обязателен.");
        }

        var activity = await dbContext.Set<Activity>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (activity is null)
        {
            throw new NotFoundException($"Активность '{id}' не найдена.");
        }

        dbContext.Set<Activity>().Remove(activity);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Удалена активность. ActivityId: {ActivityId}, Type: {Type}, ClientId: {ClientId}, DealId: {DealId}, UserId: {UserId}.",
            activity.Id,
            activity.Type,
            activity.ClientId,
            activity.DealId,
            activity.UserId);
    }

    /// <summary>
    /// Проверяет корректность данных для создания активности.
    /// </summary>
    private static void ValidateCreateRequest(CreateActivityRequest request)
    {
        ValidateCommonRequest(
            request.ActivityDateUtc,
            request.ClientId,
            request.DealId,
            request.UserId);
    }

    /// <summary>
    /// Проверяет корректность данных для обновления активности.
    /// </summary>
    private static void ValidateUpdateRequest(UpdateActivityRequest request)
    {
        ValidateCommonRequest(
            request.ActivityDateUtc,
            request.ClientId,
            request.DealId,
            request.UserId);
    }

    /// <summary>
    /// Проверяет корректность параметров списка активностей.
    /// </summary>
    private static void ValidateGetAllRequest(GetActivitiesRequest request)
    {
        if (request.ClientId.HasValue && request.ClientId.Value == Guid.Empty)
        {
            throw new ValidationException("Идентификатор клиента задан некорректно.");
        }

        if (request.DealId.HasValue && request.DealId.Value == Guid.Empty)
        {
            throw new ValidationException("Идентификатор сделки задан некорректно.");
        }

        if (request.UserId.HasValue && request.UserId.Value == Guid.Empty)
        {
            throw new ValidationException("Идентификатор пользователя задан некорректно.");
        }

        if (request.ActivityDateFromUtc.HasValue
            && request.ActivityDateToUtc.HasValue
            && request.ActivityDateFromUtc.Value > request.ActivityDateToUtc.Value)
        {
            throw new ValidationException("Начальная дата активности не может быть больше конечной.");
        }
    }

    /// <summary>
    /// Выполняет общую валидацию полей активности.
    /// </summary>
    private static void ValidateCommonRequest(
        DateTime activityDateUtc,
        Guid clientId,
        Guid? dealId,
        Guid? userId)
    {
        if (activityDateUtc == default)
        {
            throw new ValidationException("Дата активности обязательна.");
        }

        if (clientId == Guid.Empty)
        {
            throw new ValidationException("Идентификатор клиента обязателен.");
        }

        if (dealId.HasValue && dealId.Value == Guid.Empty)
        {
            throw new ValidationException("Идентификатор сделки задан некорректно.");
        }

        if (userId.HasValue && userId.Value == Guid.Empty)
        {
            throw new ValidationException("Идентификатор пользователя задан некорректно.");
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