namespace Crm.Application.Clients.DTOs;

/// <summary>
/// Параметры запроса списка клиентов.
/// </summary>
public sealed class GetClientsRequest
{
    /// <summary>
    /// Поисковая строка по имени, внешнему идентификатору или локации.
    /// </summary>
    public string? Search { get; init; }

    /// <summary>
    /// Фильтр по полу.
    /// </summary>
    public string? Gender { get; init; }

    /// <summary>
    /// Фильтр по локации.
    /// </summary>
    public string? Location { get; init; }
}
