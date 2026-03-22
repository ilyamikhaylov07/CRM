using Crm.Domain.Enums;

namespace Crm.Application.Activities.DTOs;

/// <summary>
/// Параметры запроса списка активностей.
/// </summary>
public sealed class GetActivitiesRequest
{
    /// <summary>
    /// Фильтр по клиенту.
    /// </summary>
    public Guid? ClientId { get; init; }

    /// <summary>
    /// Фильтр по сделке.
    /// </summary>
    public Guid? DealId { get; init; }

    /// <summary>
    /// Фильтр по пользователю.
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Фильтр по типу активности.
    /// </summary>
    public ActivityType? Type { get; init; }

    /// <summary>
    /// Поиск по теме или описанию.
    /// </summary>
    public string? Search { get; init; }

    /// <summary>
    /// Нижняя граница даты активности.
    /// </summary>
    public DateTime? ActivityDateFromUtc { get; init; }

    /// <summary>
    /// Верхняя граница даты активности.
    /// </summary>
    public DateTime? ActivityDateToUtc { get; init; }
}