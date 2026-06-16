// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Collections.Immutable;

namespace Yextly.OpenTelemetry;

/// <summary>
/// Immutable container for registered activity enrichers.
/// </summary>
public sealed class OtActivityEnrichers : IOtActivityEnrichers
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OtActivityEnrichers" /> class.
    /// </summary>
    /// <param name="enrichers">Registered enricher types.</param>
    public OtActivityEnrichers(ImmutableArray<Type> enrichers)
    {
        Enrichers = enrichers;
    }

    /// <inheritdoc />
    public ImmutableArray<Type> Enrichers { get; }
}
