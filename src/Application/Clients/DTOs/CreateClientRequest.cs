namespace Crm.Application.Clients.DTOs;


/// <summary>
/// Запрос на создание клиента.
/// </summary>
public sealed class CreateClientRequest
{
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
