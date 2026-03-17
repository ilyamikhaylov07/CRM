using Serilog;

namespace Crm.Infrastructure.Logging;

/// <summary>
/// Расширения для подключения Serilog логирования.
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Регистрирует Serilog как основной провайдер логирования приложения.
    /// </summary>
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
            .CreateLogger();

        builder.Host.UseSerilog();

        return builder;
    }

    /// <summary>
    /// Включает логирование HTTP-запросов через Serilog.
    /// </summary>
    public static WebApplication UseSerilogRequestLoggingMiddleware(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseSerilogRequestLogging();

        return app;
    }
}