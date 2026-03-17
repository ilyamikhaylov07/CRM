using Crm.Infrastructure.Import;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

/// <summary>
/// Контроллер импорта данных в CRM.
/// </summary>
/// <remarks>
/// Содержит endpoint для загрузки и импорта внешних данных в систему.
/// </remarks>
[ApiController]
[Route("crm/import/")]
public sealed class ImportController(ShoppingTrendsImportService importService) : ControllerBase
{
    private readonly ShoppingTrendsImportService _importService = importService;

    /// <summary>
    /// Импортирует данные из файла shopping_trends.csv в базу данных.
    /// </summary>
    /// <remarks>
    /// Использует заранее заданный путь к CSV-файлу на сервере приложения.
    /// Endpoint предназначен для технического запуска процедуры начального импорта данных.
    /// </remarks>
    /// <returns>Сообщение об успешном завершении импорта.</returns>
    /// <response code="200">Импорт успешно завершен.</response>
    /// <response code="500">Во время импорта произошла внутренняя ошибка.</response>
    [HttpPost("shopping-trends")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ImportShoppingTrends()
    {
        var filePath = @"D:\data\shopping_trends.csv";

        await _importService.ImportAsync(filePath);

        return Ok("Импорт завершён");
    }
}
