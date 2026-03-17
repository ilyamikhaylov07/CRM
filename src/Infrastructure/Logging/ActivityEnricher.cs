using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace Crm.Infrastructure.Logging;


/// <summary>
/// Добавляет TraceId и SpanId из текущей Activity в лог-событие.
/// </summary>
public sealed class ActivityEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var activity = Activity.Current;

        if (activity is null)
        {
            return;
        }

        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty("TraceId", activity.TraceId.ToString()));

        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty("SpanId", activity.SpanId.ToString()));
    }
}
