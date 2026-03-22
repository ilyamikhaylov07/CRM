namespace Crm.Application.Clients.DTOs;

/// <summary>
/// Краткая информация о клиенте для списков.
/// </summary>
public sealed class ClientListItemResponse
{
    /// <summary>
    /// Идентификатор клиента.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Внешний идентификатор клиента.
    /// </summary>
    public required string ExternalId { get; init; }

    /// <summary>
    /// Имя клиента.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Пол клиента.
    /// </summary>
    public required string Gender { get; init; }

    /// <summary>
    /// Локация клиента.
    /// </summary>
    public required string Location { get; init; }

    /// <summary>
    /// Количество предыдущих покупок.
    /// </summary>
    public int PreviousPurchases { get; init; }
}