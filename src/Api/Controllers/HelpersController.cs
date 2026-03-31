using Auth.Core.Authorization;
using Crm.Application.Helpers.RecommendationDataSeed;
using Crm.Application.Helpers.RecommendationDataSeed.DTOs;
using Crm.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

/// <summary>
/// Технический контроллер одноразовых helper-операций для наполнения тестовых данных.
/// </summary>
/// <remarks>
/// Содержит endpoint, который использует существующие данные базы и дозаполняет историю покупок
/// для более осмысленного обучения рекомендательной модели.
/// </remarks>
[ApiController]
[Route("crm/helpers")]
[AuthorizeCrm(GlobalRoles = [SystemRoles.Admin])]
public sealed class HelpersController(IRecommendationDataSeedHelper recommendationDataSeedHelper) : ControllerBase
{
    /// <summary>
    /// Генерирует дополнительную историю покупок на основе существующих клиентов и товаров.
    /// </summary>
    /// <param name="request">Параметры helper-генерации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат генерации тестовых данных.</returns>
    [HttpPost("recommendation-seed")]
    [ProducesResponseType(typeof(SeedRecommendationDataResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SeedRecommendationDataResponse>> SeedRecommendationData(
        [FromBody] SeedRecommendationDataRequest request,
        CancellationToken cancellationToken)
    {
        var response = await recommendationDataSeedHelper.SeedAsync(request, cancellationToken);
        return Ok(response);
    }
}
