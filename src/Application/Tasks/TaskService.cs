using Crm.Application.Common.Exceptions;
using Crm.Application.Tasks.DTOs;
using Crm.Domain.Entities;
using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskStatus = Crm.Domain.Enums.TaskStatus;

namespace Crm.Application.Tasks;

/// <summary>
/// Сервис управления задачами.
/// </summary>
public sealed class TaskService(
    CrmDbContext dbContext,
    ILogger<TaskService> logger) : ITaskService
{
    /// <inheritdoc />
    public async Task<Guid> CreateAsync(
        CreateTaskRequest request,
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

        User? assignedUser = null;
        if (request.AssignedToUserId.HasValue)
        {
            assignedUser = await dbContext.Set<User>()
                .FirstOrDefaultAsync(x => x.Id == request.AssignedToUserId.Value, cancellationToken);

            if (assignedUser is null)
            {
                throw new NotFoundException(
                    $"Пользователь '{request.AssignedToUserId.Value}' не найден.");
            }
        }

        var task = new TaskItem
        {
            Title = request.Title.Trim(),
            Description = NormalizeOptional(request.Description),
            DueDateUtc = request.DueDateUtc,
            Status = request.Status,
            AssignedToUserId = assignedUser?.Id,
            AssignedToUser = assignedUser,
            ClientId = client.Id,
            Client = client,
            DealId = deal?.Id,
            Deal = deal
        };

        dbContext.Set<TaskItem>().Add(task);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Создана задача. TaskId: {TaskId}, ClientId: {ClientId}, DealId: {DealId}, AssignedToUserId: {AssignedToUserId}, Status: {Status}, DueDateUtc: {DueDateUtc}.",
            task.Id,
            task.ClientId,
            task.DealId,
            task.AssignedToUserId,
            task.Status,
            task.DueDateUtc);

        return task.Id;
    }

    /// <inheritdoc />
    public async Task<TaskResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException("Идентификатор задачи обязателен.");
        }

        var task = await dbContext.Set<TaskItem>()
            .AsNoTracking()
            .Include(x => x.Client)
            .Include(x => x.AssignedToUser)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException($"Задача '{id}' не найдена.");
        }

        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDateUtc = task.DueDateUtc,
            Status = task.Status,
            AssignedToUserId = task.AssignedToUserId,
            AssignedToUserFullName = task.AssignedToUser is null
                ? null
                : $"{task.AssignedToUser.FirstName} {task.AssignedToUser.LastName}",
            ClientId = task.ClientId,
            ClientName = task.Client.Name,
            DealId = task.DealId
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<TaskListItemResponse>> GetAllAsync(
        GetTasksRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateGetAllRequest(request);

        IQueryable<TaskItem> query = dbContext.Set<TaskItem>()
            .AsNoTracking()
            .Include(x => x.Client)
            .Include(x => x.AssignedToUser);

        if (request.ClientId.HasValue)
        {
            query = query.Where(x => x.ClientId == request.ClientId.Value);
        }

        if (request.DealId.HasValue)
        {
            query = query.Where(x => x.DealId == request.DealId.Value);
        }

        if (request.AssignedToUserId.HasValue)
        {
            query = query.Where(x => x.AssignedToUserId == request.AssignedToUserId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                x.Title.Contains(search) ||
                (x.Description != null && x.Description.Contains(search)));
        }

        if (request.DueDateFromUtc.HasValue)
        {
            query = query.Where(x => x.DueDateUtc >= request.DueDateFromUtc.Value);
        }

        if (request.DueDateToUtc.HasValue)
        {
            query = query.Where(x => x.DueDateUtc <= request.DueDateToUtc.Value);
        }

        var tasks = await query
            .OrderBy(x => x.DueDateUtc)
            .ThenBy(x => x.Title)
            .Select(x => new TaskListItemResponse
            {
                Id = x.Id,
                Title = x.Title,
                DueDateUtc = x.DueDateUtc,
                Status = x.Status,
                AssignedToUserId = x.AssignedToUserId,
                AssignedToUserFullName = x.AssignedToUser == null
                    ? null
                    : x.AssignedToUser.FirstName + " " + x.AssignedToUser.LastName,
                ClientId = x.ClientId,
                ClientName = x.Client.Name,
                DealId = x.DealId
            })
            .ToListAsync(cancellationToken);

        return tasks;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(
        Guid id,
        UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (id == Guid.Empty)
        {
            throw new ValidationException("Идентификатор задачи обязателен.");
        }

        ValidateUpdateRequest(request);

        var task = await dbContext.Set<TaskItem>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException($"Задача '{id}' не найдена.");
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

        User? assignedUser = null;
        if (request.AssignedToUserId.HasValue)
        {
            assignedUser = await dbContext.Set<User>()
                .FirstOrDefaultAsync(x => x.Id == request.AssignedToUserId.Value, cancellationToken);

            if (assignedUser is null)
            {
                throw new NotFoundException(
                    $"Пользователь '{request.AssignedToUserId.Value}' не найден.");
            }
        }

        var oldTitle = task.Title;
        var oldDueDateUtc = task.DueDateUtc;
        var oldStatus = task.Status;
        var oldAssignedToUserId = task.AssignedToUserId;
        var oldClientId = task.ClientId;
        var oldDealId = task.DealId;

        task.Title = request.Title.Trim();
        task.Description = NormalizeOptional(request.Description);
        task.DueDateUtc = request.DueDateUtc;
        task.Status = request.Status;
        task.AssignedToUserId = assignedUser?.Id;
        task.AssignedToUser = assignedUser;
        task.ClientId = client.Id;
        task.Client = client;
        task.DealId = deal?.Id;
        task.Deal = deal;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Обновлена задача. TaskId: {TaskId}, OldTitle: {OldTitle}, NewTitle: {NewTitle}, OldDueDateUtc: {OldDueDateUtc}, NewDueDateUtc: {NewDueDateUtc}, OldStatus: {OldStatus}, NewStatus: {NewStatus}, OldAssignedToUserId: {OldAssignedToUserId}, NewAssignedToUserId: {NewAssignedToUserId}, OldClientId: {OldClientId}, NewClientId: {NewClientId}, OldDealId: {OldDealId}, NewDealId: {NewDealId}.",
            task.Id,
            oldTitle,
            task.Title,
            oldDueDateUtc,
            task.DueDateUtc,
            oldStatus,
            task.Status,
            oldAssignedToUserId,
            task.AssignedToUserId,
            oldClientId,
            task.ClientId,
            oldDealId,
            task.DealId);
    }

    /// <inheritdoc />
    public async Task ChangeStatusAsync(
        Guid id,
        ChangeTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (id == Guid.Empty)
        {
            throw new ValidationException("Идентификатор задачи обязателен.");
        }

        var task = await dbContext.Set<TaskItem>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException($"Задача '{id}' не найдена.");
        }

        if (task.Status == request.Status)
        {
            return;
        }

        var oldStatus = task.Status;
        task.Status = request.Status;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Изменен статус задачи. TaskId: {TaskId}, OldStatus: {OldStatus}, NewStatus: {NewStatus}.",
            task.Id,
            oldStatus,
            task.Status);
    }

    /// <inheritdoc />
    public async Task AssignAsync(
        Guid id,
        AssignTaskRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (id == Guid.Empty)
        {
            throw new ValidationException("Идентификатор задачи обязателен.");
        }

        var task = await dbContext.Set<TaskItem>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException($"Задача '{id}' не найдена.");
        }

        User? assignedUser = null;
        if (request.AssignedToUserId.HasValue)
        {
            if (request.AssignedToUserId.Value == Guid.Empty)
            {
                throw new ValidationException("Идентификатор исполнителя задан некорректно.");
            }

            assignedUser = await dbContext.Set<User>()
                .FirstOrDefaultAsync(x => x.Id == request.AssignedToUserId.Value, cancellationToken);

            if (assignedUser is null)
            {
                throw new NotFoundException(
                    $"Пользователь '{request.AssignedToUserId.Value}' не найден.");
            }
        }

        var oldAssignedToUserId = task.AssignedToUserId;

        if (oldAssignedToUserId == assignedUser?.Id)
        {
            return;
        }

        task.AssignedToUserId = assignedUser?.Id;
        task.AssignedToUser = assignedUser;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Изменен исполнитель задачи. TaskId: {TaskId}, OldAssignedToUserId: {OldAssignedToUserId}, NewAssignedToUserId: {NewAssignedToUserId}.",
            task.Id,
            oldAssignedToUserId,
            task.AssignedToUserId);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException("Идентификатор задачи обязателен.");
        }

        var task = await dbContext.Set<TaskItem>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException($"Задача '{id}' не найдена.");
        }

        dbContext.Set<TaskItem>().Remove(task);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Удалена задача. TaskId: {TaskId}, ClientId: {ClientId}, DealId: {DealId}, AssignedToUserId: {AssignedToUserId}, Status: {Status}.",
            task.Id,
            task.ClientId,
            task.DealId,
            task.AssignedToUserId,
            task.Status);
    }

    /// <summary>
    /// Проверяет корректность данных для создания задачи.
    /// </summary>
    private static void ValidateCreateRequest(CreateTaskRequest request)
    {
        ValidateCommonRequest(request.Title, request.DueDateUtc, request.ClientId, request.DealId, request.AssignedToUserId);
    }

    /// <summary>
    /// Проверяет корректность данных для обновления задачи.
    /// </summary>
    private static void ValidateUpdateRequest(UpdateTaskRequest request)
    {
        ValidateCommonRequest(request.Title, request.DueDateUtc, request.ClientId, request.DealId, request.AssignedToUserId);
    }

    /// <summary>
    /// Проверяет корректность параметров списка задач.
    /// </summary>
    private static void ValidateGetAllRequest(GetTasksRequest request)
    {
        if (request.ClientId.HasValue && request.ClientId.Value == Guid.Empty)
        {
            throw new ValidationException("Идентификатор клиента задан некорректно.");
        }

        if (request.DealId.HasValue && request.DealId.Value == Guid.Empty)
        {
            throw new ValidationException("Идентификатор сделки задан некорректно.");
        }

        if (request.AssignedToUserId.HasValue && request.AssignedToUserId.Value == Guid.Empty)
        {
            throw new ValidationException("Идентификатор исполнителя задан некорректно.");
        }

        if (request.DueDateFromUtc.HasValue
            && request.DueDateToUtc.HasValue
            && request.DueDateFromUtc.Value > request.DueDateToUtc.Value)
        {
            throw new ValidationException("Начальная дата срока не может быть больше конечной.");
        }
    }

    /// <summary>
    /// Выполняет общую валидацию полей задачи.
    /// </summary>
    private static void ValidateCommonRequest(
        string title,
        DateTime dueDateUtc,
        Guid clientId,
        Guid? dealId,
        Guid? assignedToUserId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ValidationException("Заголовок задачи обязателен.");
        }

        if (dueDateUtc == default)
        {
            throw new ValidationException("Срок выполнения задачи обязателен.");
        }

        if (clientId == Guid.Empty)
        {
            throw new ValidationException("Идентификатор клиента обязателен.");
        }

        if (dealId.HasValue && dealId.Value == Guid.Empty)
        {
            throw new ValidationException("Идентификатор сделки задан некорректно.");
        }

        if (assignedToUserId.HasValue && assignedToUserId.Value == Guid.Empty)
        {
            throw new ValidationException("Идентификатор исполнителя задан некорректно.");
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