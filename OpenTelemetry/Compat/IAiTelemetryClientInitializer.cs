using System.Diagnostics;

namespace Yextly.OpenTelemetry.Azure.Compat;

/// <summary>
/// Legacy initializer contract retained for migration convenience.
/// </summary>
[Obsolete("Use IOpenTelemetryActivityEnricher instead.")]
public interface IAiTelemetryClientInitializer : IOpenTelemetryActivityEnricher
{
    void IOpenTelemetryActivityEnricher.Enrich(Activity activity)
    {
        Initialize(activity);
    }

    /// <summary>
    /// Initializes the supplied activity.
    /// </summary>
    /// <param name="activity">Activity to initialize.</param>
    void Initialize(Activity activity);
}
