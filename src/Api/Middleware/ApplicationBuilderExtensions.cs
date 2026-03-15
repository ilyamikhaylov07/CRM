namespace Crm.Api.Middleware;


/// <summary>
/// Расширения для регистрации middleware обработки ошибок.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Добавляет middleware глобальной обработки исключений.
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandling(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
