using System.Net;
using System.Text.Json;

namespace Crm.Api.Middleware;

/// <summary>
/// Глобальный middleware для перехвата необработанных исключений
/// и формирования единообразного HTTP-ответа.
/// </summary>
public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    /// <summary>
    /// Обрабатывает HTTP-запрос и перехватывает исключения.
    /// </summary>
    /// <param name="context">Контекст HTTP-запроса.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Произошла необработанная ошибка.");

            await HandleExceptionAsync(context, exception);
        }
    }

    /// <summary>
    /// Формирует HTTP-ответ на основе типа исключения.
    /// </summary>
    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = exception switch
        {
            ArgumentException => HttpStatusCode.BadRequest,
            InvalidOperationException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            KeyNotFoundException => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new ErrorResponse
        {
            Status = context.Response.StatusCode,
            Message = exception.Message
        };

        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }

    /// <summary>
    /// DTO ответа ошибки.
    /// </summary>
    private sealed record ErrorResponse
    {
        /// <summary>
        /// HTTP статус ошибки.
        /// </summary>
        public int Status { get; init; }

        /// <summary>
        /// Сообщение ошибки.
        /// </summary>
        public string Message { get; init; } = string.Empty;
    }
}