using System.Diagnostics;

namespace Yextly.OpenTelemetry.Azure.Compat;

/// <summary>
/// Legacy telemetry client wrapper retained for migration convenience.
/// </summary>
[Obsolete("Use OpenTelemetryTelemetryClient instead.")]
public sealed class AiTelemetryClient : OpenTelemetryTelemetryClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AiTelemetryClient" /> class.
    /// </summary>
    public AiTelemetryClient(ActivitySource activitySource, IOpenTelemetryActivityEnrichers enrichers, IServiceProvider serviceProvider)
        : base(activitySource, enrichers, serviceProvider)
    {
    }
}
