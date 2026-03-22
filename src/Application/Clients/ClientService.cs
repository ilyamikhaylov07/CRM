using Crm.Application.Clients.DTOs;
using Crm.Application.Common.Exceptions;
using Crm.Domain.Entities;
using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Crm.Application.Clients;

/// <summary>
/// Сервис управления клиентами.
/// </summary>
public sealed class ClientService(
    CrmDbContext dbContext,
    ILogger<ClientService> logger) : IClientService
{
    /// <inheritdoc />
    public async Task<Guid> CreateAsync(
        CreateClientRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateCreateRequest(request);

        var normalizedExternalId = request.ExternalId.Trim();

        var externalIdExists = await dbContext.Set<Client>()
            .AnyAsync(x => x.ExternalId == normalizedExternalId, cancellationToken);

        if (externalIdExists)
        {
            logger.LogWarning(
                "Попытка создать клиента с уже существующим ExternalId '{ExternalId}'.",
                normalizedExternalId);

            throw new ConflictException(
                $"Клиент с внешним идентификатором '{normalizedExternalId}' уже существует.");
        }

        var client = new Client
        {
            ExternalId = normalizedExternalId,
            Name = request.Name.Trim(),
            Age = request.Age,
            Gender = request.Gender.Trim(),
            Location = request.Location.Trim(),
            PreviousPurchases = request.PreviousPurchases,
            FrequencyOfPurchases = request.FrequencyOfPurchases.Trim()
        };

        dbContext.Set<Client>().Add(client);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Создан клиент. ClientId: {ClientId}, ExternalId: {ExternalId}, Name: {Name}.",
            client.Id,
            client.ExternalId,
            client.Name);

        return client.Id;
    }

    /// <inheritdoc />
    public async Task<ClientResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var client = await dbContext.Set<Client>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (client is null)
        {
            throw new NotFoundException($"Клиент '{id}' не найден.");
        }

        return new ClientResponse
        {
            Id = client.Id,
            ExternalId = client.ExternalId,
            Name = client.Name,
            Age = client.Age,
            Gender = client.Gender,
            Location = client.Location,
            PreviousPurchases = client.PreviousPurchases,
            FrequencyOfPurchases = client.FrequencyOfPurchases
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ClientListItemResponse>> GetAllAsync(
        GetClientsRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        IQueryable<Client> query = dbContext.Set<Client>()
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                x.Name.Contains(search) ||
                x.ExternalId.Contains(search) ||
                x.Location.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(request.Gender))
        {
            var gender = request.Gender.Trim();

            query = query.Where(x => x.Gender == gender);
        }

        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            var location = request.Location.Trim();

            query = query.Where(x => x.Location.Contains(location));
        }

        var clients = await query
            .OrderBy(x => x.Name)
            .Select(x => new ClientListItemResponse
            {
                Id = x.Id,
                ExternalId = x.ExternalId,
                Name = x.Name,
                Gender = x.Gender,
                Location = x.Location,
                PreviousPurchases = x.PreviousPurchases
            })
            .ToListAsync(cancellationToken);

        return clients;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(
        Guid id,
        UpdateClientRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateUpdateRequest(request);

        var client = await dbContext.Set<Client>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (client is null)
        {
            throw new NotFoundException($"Клиент '{id}' не найден.");
        }

        var oldName = client.Name;
        var oldLocation = client.Location;
        var oldPreviousPurchases = client.PreviousPurchases;

        client.Name = request.Name.Trim();
        client.Age = request.Age;
        client.Gender = request.Gender.Trim();
        client.Location = request.Location.Trim();
        client.PreviousPurchases = request.PreviousPurchases;
        client.FrequencyOfPurchases = request.FrequencyOfPurchases.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Обновлен клиент. ClientId: {ClientId}, OldName: {OldName}, NewName: {NewName}, OldLocation: {OldLocation}, NewLocation: {NewLocation}, OldPreviousPurchases: {OldPreviousPurchases}, NewPreviousPurchases: {NewPreviousPurchases}.",
            client.Id,
            oldName,
            client.Name,
            oldLocation,
            client.Location,
            oldPreviousPurchases,
            client.PreviousPurchases);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var client = await dbContext.Set<Client>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (client is null)
        {
            throw new NotFoundException($"Клиент '{id}' не найден.");
        }

        var hasDeals = await dbContext.Set<Deal>()
            .AnyAsync(x => x.ClientId == id, cancellationToken);

        var hasActivities = await dbContext.Set<Activity>()
            .AnyAsync(x => x.ClientId == id, cancellationToken);

        var hasTasks = await dbContext.Set<TaskItem>()
            .AnyAsync(x => x.ClientId == id, cancellationToken);

        if (hasDeals || hasActivities || hasTasks)
        {
            logger.LogWarning(
                "Запрещено удаление клиента. ClientId: {ClientId}, HasDeals: {HasDeals}, HasActivities: {HasActivities}, HasTasks: {HasTasks}.",
                id,
                hasDeals,
                hasActivities,
                hasTasks);

            throw new ConflictException(
                $"Невозможно удалить клиента '{id}', так как с ним связаны сделки, активности или задачи.");
        }

        dbContext.Set<Client>().Remove(client);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Удален клиент. ClientId: {ClientId}, ExternalId: {ExternalId}, Name: {Name}.",
            client.Id,
            client.ExternalId,
            client.Name);
    }

    /// <summary>
    /// Проверяет корректность данных для создания клиента.
    /// </summary>
    private static void ValidateCreateRequest(CreateClientRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ExternalId))
        {
            throw new ValidationException("Внешний идентификатор клиента обязателен.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException("Имя клиента обязательно.");
        }

        if (request.Age < 0 || request.Age > 120)
        {
            throw new ValidationException("Возраст клиента должен быть в диапазоне от 0 до 120.");
        }

        if (string.IsNullOrWhiteSpace(request.Gender))
        {
            throw new ValidationException("Пол клиента обязателен.");
        }

        if (string.IsNullOrWhiteSpace(request.Location))
        {
            throw new ValidationException("Локация клиента обязательна.");
        }

        if (request.PreviousPurchases < 0)
        {
            throw new ValidationException("Количество предыдущих покупок не может быть отрицательным.");
        }

        if (string.IsNullOrWhiteSpace(request.FrequencyOfPurchases))
        {
            throw new ValidationException("Частота покупок обязательна.");
        }
    }

    /// <summary>
    /// Проверяет корректность данных для обновления клиента.
    /// </summary>
    private static void ValidateUpdateRequest(UpdateClientRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException("Имя клиента обязательно.");
        }

        if (request.Age < 0 || request.Age > 120)
        {
            throw new ValidationException("Возраст клиента должен быть в диапазоне от 0 до 120.");
        }

        if (string.IsNullOrWhiteSpace(request.Gender))
        {
            throw new ValidationException("Пол клиента обязателен.");
        }

        if (string.IsNullOrWhiteSpace(request.Location))
        {
            throw new ValidationException("Локация клиента обязательна.");
        }

        if (request.PreviousPurchases < 0)
        {
            throw new ValidationException("Количество предыдущих покупок не может быть отрицательным.");
        }

        if (string.IsNullOrWhiteSpace(request.FrequencyOfPurchases))
        {
            throw new ValidationException("Частота покупок обязательна.");
        }
    }
}
