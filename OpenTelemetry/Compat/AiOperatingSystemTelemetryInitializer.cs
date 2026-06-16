using System.Diagnostics;

namespace Yextly.OpenTelemetry.Azure.Compat;

/// <summary>
/// Legacy operating system initializer retained for migration convenience.
/// </summary>
[Obsolete("Use OperatingSystemActivityEnricher instead.")]
public sealed class AiOperatingSystemTelemetryInitializer : IAiTelemetryClientInitializer
{
    private readonly OperatingSystemActivityEnricher _inner = new();

    /// <inheritdoc />
    public void Initialize(Activity activity)
    {
        _inner.Enrich(activity);
    }
}
