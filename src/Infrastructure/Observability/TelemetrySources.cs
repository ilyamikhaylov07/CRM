using System.Diagnostics;

namespace Crm.Infrastructure.Observability;

/// <summary>
/// Источники телеметрии приложения.
/// </summary>
public static class TelemetrySources
{
    /// <summary>
    /// Источник activity для пользовательских трассировок CRM API.
    /// </summary>
    public static readonly ActivitySource ActivitySource = new("Crm.Api");
}
