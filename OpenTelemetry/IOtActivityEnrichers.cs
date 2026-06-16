// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Collections.Immutable;

namespace Yextly.OpenTelemetry;

/// <summary>
/// Provides the activity enricher types registered for this telemetry pipeline.
/// </summary>
public interface IOtActivityEnrichers
{
    /// <summary>
    /// Gets the registered enricher types.
    /// </summary>
    ImmutableArray<Type> Enrichers { get; }
}
