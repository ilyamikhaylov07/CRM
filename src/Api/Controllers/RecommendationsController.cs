using Auth.Core.Authorization;
using Crm.Application.Recommendations;
using Crm.Application.Recommendations.DTOs;
using Crm.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

/// <summary>
/// Контроллер работы с сохранёнными рекомендациями.
/// </summary>
/// <remarks>
/// Предоставляет операции получения и изменения статуса рекомендаций.
/// </remarks>
[ApiController]
[Route("crm/recommendations")]
[AuthorizeCrm(GlobalRoles = [SystemRoles.Admin, SystemRoles.Manager, SystemRoles.HeadManager])]
public sealed class RecommendationsController(IRecommendationService recommendationService) : ControllerBase
{
    /// <summary>
    /// Получает список рекомендаций с учетом фильтров.
    /// </summary>
    /// <param name="request">Параметры фильтрации списка рекомендаций.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция рекомендаций.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<RecommendationListItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<RecommendationListItemResponse>>> GetAll(
        [FromQuery] GetRecommendationsRequest request,
        CancellationToken cancellationToken)
    {
        var items = await recommendationService.GetAllAsync(request, cancellationToken);
        return Ok(items);
    }

    /// <summary>
    /// Получает рекомендацию по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор рекомендации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные рекомендации.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RecommendationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecommendationResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await recommendationService.GetByIdAsync(id, cancellationToken);
        return Ok(item);
    }

    /// <summary>
    /// Получает рекомендации конкретного клиента.
    /// </summary>
    /// <param name="clientId">Идентификатор клиента.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция рекомендаций клиента.</returns>
    [HttpGet("by-client/{clientId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<RecommendationListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<RecommendationListItemResponse>>> GetByClientId(
        Guid clientId,
        CancellationToken cancellationToken)
    {
        var items = await recommendationService.GetByClientIdAsync(clientId, cancellationToken);
        return Ok(items);
    }

    /// <summary>
    /// Обновляет статус рекомендации.
    /// </summary>
    /// <param name="id">Идентификатор рекомендации.</param>
    /// <param name="request">Новый статус рекомендации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешном обновлении.</returns>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] ChangeRecommendationStatusRequest request,
        CancellationToken cancellationToken)
    {
        await recommendationService.UpdateStatusAsync(id, request, cancellationToken);
        return NoContent();
    }
}
