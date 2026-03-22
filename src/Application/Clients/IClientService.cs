using Crm.Application.Clients.DTOs;

namespace Crm.Application.Clients;

/// <summary>
/// Сервис управления клиентами.
/// </summary>
public interface IClientService
{
    /// <summary>
    /// Создает нового клиента.
    /// </summary>
    Task<Guid> CreateAsync(CreateClientRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает клиента по идентификатору.
    /// </summary>
    Task<ClientResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает список клиентов с учетом фильтров.
    /// </summary>
    Task<IReadOnlyCollection<ClientListItemResponse>> GetAllAsync(
        GetClientsRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет данные клиента.
    /// </summary>
    Task UpdateAsync(Guid id, UpdateClientRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет клиента.
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
