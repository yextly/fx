namespace Yextly.OpenTelemetry.Azure.Compat;

/// <summary>
/// Legacy no-op telemetry client retained for migration convenience.
/// </summary>
[Obsolete("Use NoTelemetryClient instead.")]
public sealed class AiNoTelemetryClient : NoTelemetryClient
{
}
