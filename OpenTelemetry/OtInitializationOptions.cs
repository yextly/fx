// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Collections.Immutable;

namespace Yextly.OpenTelemetry;

/// <summary>
/// Initialization options for OpenTelemetry-based telemetry support.
/// </summary>
public sealed class OtInitializationOptions
{
    private ImmutableArray<Type>.Builder _types = ImmutableArray.CreateBuilder<Type>();

    /// <summary>
    /// Gets or sets the activity source name used to create operations.
    /// </summary>
    public string ActivitySourceName { get; set; } = "Yextly.OpenTelemetry";

    /// <summary>
    /// Gets or sets a value indicating whether telemetry is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets the registered activity enricher types.
    /// </summary>
    public ImmutableArray<Type> Enrichers => _types.ToImmutable();

    /// <summary>
    /// Adds an activity enricher.
    /// </summary>
    /// <typeparam name="T">The enricher type.</typeparam>
    /// <returns>The current options instance.</returns>
    public OtInitializationOptions AddEnricher<T>()
        where T : class, IOtActivityEnricher
    {
        _types.Add(typeof(T));
        return this;
    }

    internal OtInitializationOptions Clone()
    {
        var clone = new OtInitializationOptions
        {
            Enabled = Enabled,
            ActivitySourceName = ActivitySourceName,
            _types = ImmutableArray.CreateBuilder<Type>()
        };

        clone._types.AddRange(_types);
        return clone;
    }
}
