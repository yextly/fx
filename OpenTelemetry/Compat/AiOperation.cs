using System.Diagnostics;

namespace Yextly.OpenTelemetry.Azure.Compat;

/// <summary>
/// Legacy operation wrapper retained for migration convenience.
/// </summary>
[Obsolete("Use OpenTelemetryOperation instead.")]
public sealed class AiOperation : OpenTelemetryOperation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AiOperation"/> class.
    /// </summary>
    public AiOperation(OpenTelemetryTelemetryClient client, Activity? activity)
        : base(client, activity)
    {
    }
}
