using System.Collections.Immutable;

namespace Yextly.OpenTelemetry.Azure.Compat;

/// <summary>
/// Backward-compatible options wrapper for migration from TelemetryAzure.
/// </summary>
[Obsolete("Use OpenTelemetryInitializationOptions instead.")]
public sealed class AiTelemetryInitializationOptions
{
    private readonly ImmutableArray<Type>.Builder _types = ImmutableArray.CreateBuilder<Type>();

    /// <summary>
    /// Gets or sets the activity source name.
    /// </summary>
    public string ActivitySourceName { get; set; } = "Yextly.OpenTelemetry.Azure";

    /// <summary>
    /// Gets or sets a value indicating whether telemetry is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets the registered initializer types.
    /// </summary>
    public ImmutableArray<Type> Initializers => _types.ToImmutable();

    /// <summary>
    /// Adds a legacy initializer type.
    /// </summary>
    public AiTelemetryInitializationOptions AddInitializer<T>()
        where T : class, IAiTelemetryClientInitializer
    {
        _types.Add(typeof(T));
        return this;
    }
}
