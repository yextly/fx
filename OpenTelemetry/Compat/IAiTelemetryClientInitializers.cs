using System.Collections.Immutable;

namespace Yextly.OpenTelemetry.Azure.Compat;

/// <summary>
/// Legacy initializer registry retained for migration convenience.
/// </summary>
[Obsolete("Use IOpenTelemetryActivityEnrichers instead.")]
public interface IAiTelemetryClientInitializers
{
    /// <summary>
    /// Gets the registered initializer types.
    /// </summary>
    ImmutableArray<Type> Initializers { get; }
}
