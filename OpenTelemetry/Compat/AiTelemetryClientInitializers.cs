using System.Collections.Immutable;

namespace Yextly.OpenTelemetry.Azure.Compat;

/// <summary>
/// Legacy initializer registry retained for migration convenience.
/// </summary>
[Obsolete("Use OpenTelemetryActivityEnrichers instead.")]
public sealed class AiTelemetryClientInitializers : IAiTelemetryClientInitializers
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AiTelemetryClientInitializers" /> class.
    /// </summary>
    public AiTelemetryClientInitializers(ImmutableArray<Type> initializers)
    {
        Initializers = initializers;
    }

    /// <inheritdoc />
    public ImmutableArray<Type> Initializers { get; }
}
