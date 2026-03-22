namespace Crm.Application.Clients.DTOs;

/// <summary>
/// Полная информация о клиенте.
/// </summary>
public sealed class ClientResponse
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
    /// Возраст клиента.
    /// </summary>
    public int Age { get; init; }

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

    /// <summary>
    /// Частота покупок.
    /// </summary>
    public required string FrequencyOfPurchases { get; init; }
}
