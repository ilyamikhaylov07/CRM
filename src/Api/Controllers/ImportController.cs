using Crm.Infrastructure.Import;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("[controller]/[action]/Import")]
public sealed class ImportController(ShoppingTrendsImportService importService) : ControllerBase
{
    private readonly ShoppingTrendsImportService _importService = importService;

    [HttpPost("ShoppingTrends")]
    public async Task<IActionResult> ImportShoppingTrends()
    {
        var filePath = @"D:\data\shopping_trends.csv";

        await _importService.ImportAsync(filePath);

        return Ok("Импорт завершён");
    }
}
