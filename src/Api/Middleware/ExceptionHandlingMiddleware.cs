using Crm.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace Crm.Api.Middleware;

/// <summary>
/// Глобальный middleware для централизованного перехвата исключений
/// и формирования единообразного HTTP-ответа.
/// </summary>
public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    /// <summary>
    /// Обрабатывает HTTP-запрос и централизованно перехватывает исключения.
    /// </summary>
    /// <param name="context">Контекст HTTP-запроса.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            logger.LogInformation(
                "HTTP-запрос был отменен клиентом. TraceId: {TraceId}",
                context.TraceIdentifier);

            throw;
        }
        catch (Exception exception)
        {
            var (statusCode, logLevel, includeExceptionDetails) = exception switch
            {
                ValidationException => (HttpStatusCode.BadRequest, LogLevel.Warning, false),
                AuthenticationException => (HttpStatusCode.Unauthorized, LogLevel.Warning, false),
                AuthorizationException => (HttpStatusCode.Forbidden, LogLevel.Warning, false),
                NotFoundException => (HttpStatusCode.NotFound, LogLevel.Warning, false),
                ConflictException => (HttpStatusCode.Conflict, LogLevel.Warning, false),

                ExternalServiceException => (HttpStatusCode.BadGateway, LogLevel.Error, true),

                _ => (HttpStatusCode.InternalServerError, LogLevel.Error, true)
            };

            if (includeExceptionDetails)
            {
                logger.Log(
                    logLevel,
                    exception,
                    "Во время обработки HTTP-запроса возникло исключение. TraceId: {TraceId}",
                    context.TraceIdentifier);
            }
            else
            {
                logger.Log(
                    logLevel,
                    "Ошибка запроса. StatusCode: {StatusCode}. Message: {Message}. TraceId: {TraceId}",
                    (int)statusCode,
                    exception.Message,
                    context.TraceIdentifier);
            }

            await HandleExceptionAsync(context, exception, statusCode);
        }
    }

    /// <summary>
    /// Формирует HTTP-ответ на основе типа исключения.
    /// </summary>
    /// <param name="context">Контекст HTTP-запроса.</param>
    /// <param name="exception">Перехваченное исключение.</param>
    /// <param name="statusCode">HTTP-статус ответа.</param>
    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        HttpStatusCode statusCode)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new ErrorResponse
        {
            Status = context.Response.StatusCode,
            Message = exception.Message,
            TraceId = context.TraceIdentifier
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
        /// HTTP-статус ошибки.
        /// </summary>
        public int Status { get; init; }

        /// <summary>
        /// Сообщение об ошибке.
        /// </summary>
        public string Message { get; init; } = string.Empty;

        /// <summary>
        /// Идентификатор трассировки запроса.
        /// </summary>
        public string TraceId { get; init; } = string.Empty;
    }
}