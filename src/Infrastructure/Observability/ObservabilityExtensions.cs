using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Crm.Infrastructure.Observability;

/// <summary>
/// Расширения для подключения OpenTelemetry tracing.
/// </summary>
public static class ObservabilityExtensions
{
    /// <summary>
    /// Регистрирует OpenTelemetry tracing для приложения.
    /// </summary>
    public static IServiceCollection AddCrmTracing(
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
                            !httpContext.Request.Path.StartsWithSegments("/swagger");
                    })
                    .AddHttpClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                    })
                    .AddSource("Crm.Api");

                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                {
                    tracing.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                    });
                }
            });

        return services;
    }
}
