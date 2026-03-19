using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Crm.Infrastructure.Observability;

/// <summary>
/// Расширения для подключения OpenTelemetry observability.
/// </summary>
public static class ObservabilityExtensions
{
    /// <summary>
    /// Регистрирует OpenTelemetry tracing и metrics для приложения.
    /// </summary>
    public static IServiceCollection AddCrmObservability(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName,
        string serviceVersion)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceVersion);

        var otlpEndpoint = configuration["OpenTelemetry:Otlp:Endpoint"];

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(
                serviceName: serviceName,
                serviceVersion: serviceVersion))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;

                        options.Filter = httpContext =>
                            !httpContext.Request.Path.StartsWithSegments("/swagger") &&
                            !httpContext.Request.Path.StartsWithSegments("/metrics");
                    })
                    .AddHttpClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                    })
                    .AddSource(ObservabilityConstants.ActivitySourceName);

                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                {
                    tracing.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                    });
                }
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter(ObservabilityConstants.MeterName)
                    .AddPrometheusExporter();           
            });

        return services;
    }

    /// <summary>
    /// Подключает endpoint Prometheus для сбора метрик.
    /// </summary>
    public static IApplicationBuilder UseCrmObservability(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseOpenTelemetryPrometheusScrapingEndpoint();

        return app;
    }
}
