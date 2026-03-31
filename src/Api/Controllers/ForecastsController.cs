using Auth.Core.Authorization;
using Crm.Application.Forecasts;
using Crm.Application.Forecasts.DTOs;
using Crm.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

/// <summary>
/// Контроллер работы с сохранёнными прогнозами продаж.
/// </summary>
/// <remarks>
/// Предоставляет операции получения сохранённых прогнозов.
/// </remarks>
[ApiController]
[Route("crm/forecasts")]
[AuthorizeCrm(GlobalRoles = [SystemRoles.Admin, SystemRoles.Manager, SystemRoles.HeadManager])]
public sealed class ForecastsController(IForecastService forecastService) : ControllerBase
{
    /// <summary>
    /// Получает список прогнозов с учетом фильтров.
    /// </summary>
    /// <param name="request">Параметры фильтрации списка прогнозов.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция прогнозов.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ForecastListItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ForecastListItemResponse>>> GetAll(
        [FromQuery] GetForecastsRequest request,
        CancellationToken cancellationToken)
    {
        var items = await forecastService.GetAllAsync(request, cancellationToken);
        return Ok(items);
    }

    /// <summary>
    /// Получает прогноз по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор прогноза.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные прогноза.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ForecastResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ForecastResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await forecastService.GetByIdAsync(id, cancellationToken);
        return Ok(item);
    }

    /// <summary>
    /// Получает прогнозы конкретного клиента.
    /// </summary>
    /// <param name="clientId">Идентификатор клиента.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция прогнозов клиента.</returns>
    [HttpGet("by-client/{clientId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ForecastListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<ForecastListItemResponse>>> GetByClientId(
        Guid clientId,
        CancellationToken cancellationToken)
    {
        var items = await forecastService.GetByClientIdAsync(clientId, cancellationToken);
        return Ok(items);
    }
}
