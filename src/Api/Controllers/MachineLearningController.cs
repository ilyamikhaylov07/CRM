using Auth.Core.Authorization;
using Crm.Application.ML;
using Crm.Application.ML.DTOs;
using Crm.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

/// <summary>
/// Контроллер для управления ML-процессами: обучение модели, генерация прогнозов и рекомендаций.
/// </summary>
[ApiController]
[Route("crm/ml")]
[AuthorizeCrm(GlobalRoles = [SystemRoles.Admin, SystemRoles.HeadManager])]
public sealed class MachineLearningController(IMachineLearningOrchestrator orchestrator) : ControllerBase
{
    /// <summary>
    /// Запускает полный ML-сценарий: обучение модели, генерацию прогнозов и рекомендаций.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Сводный результат выполнения ML-процессов.</returns>
    [HttpPost("run")]
    [ProducesResponseType(typeof(RunTrainingResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<RunTrainingResponse>> Run(CancellationToken cancellationToken)
    {
        return Ok(await orchestrator.RunAsync(cancellationToken));
    }

    /// <summary>
    /// Генерирует прогнозы продаж для клиентов.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция прогнозов продаж.</returns>
    [HttpPost("forecasts")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ForecastResultResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ForecastResultResponse>>> GenerateForecasts(CancellationToken cancellationToken)
    {
        return Ok(await orchestrator.GenerateForecastsAsync(cancellationToken));
    }

    /// <summary>
    /// Генерирует рекомендации товаров для клиентов.
    /// </summary>
    /// <param name="topPerClient">Количество рекомендаций на одного клиента.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция рекомендаций.</returns>
    [HttpPost("recommendations")]
    [ProducesResponseType(typeof(IReadOnlyCollection<RecommendationResultResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<RecommendationResultResponse>>> GenerateRecommendations(
        [FromQuery] int topPerClient = 3,
        CancellationToken cancellationToken = default)
    {
        return Ok(await orchestrator.GenerateRecommendationsAsync(topPerClient, cancellationToken));
    }
}
