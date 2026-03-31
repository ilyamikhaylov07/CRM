using Crm.Application.Forecasts.DTOs;

namespace Crm.Application.Forecasts;

/// <summary>
/// Сервис работы с сохранёнными прогнозами продаж.
/// </summary>
public interface IForecastService
{
    /// <summary>
    /// Возвращает список прогнозов с учетом фильтров.
    /// </summary>
    /// <param name="request">Параметры фильтрации списка прогнозов.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция прогнозов.</returns>
    Task<IReadOnlyCollection<ForecastListItemResponse>> GetAllAsync(
        GetForecastsRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает прогноз по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор прогноза.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные прогноза.</returns>
    Task<ForecastResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает прогнозы конкретного клиента.
    /// </summary>
    /// <param name="clientId">Идентификатор клиента.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция прогнозов клиента.</returns>
    Task<IReadOnlyCollection<ForecastListItemResponse>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken);
}
