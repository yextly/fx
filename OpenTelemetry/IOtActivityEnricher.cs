// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Diagnostics;

namespace Yextly.OpenTelemetry;

/// <summary>
/// Enriches an <see cref="Activity" /> created by <see cref="OtTelemetryClient" />.
/// </summary>
public interface IOtActivityEnricher
{
    /// <summary>
    /// Enriches the supplied <paramref name="activity" />.
    /// </summary>
    /// <param name="activity">The activity being initialized.</param>
    void Enrich(Activity activity);
}
