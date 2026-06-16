// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Yextly.Telemetry.Abstractions;

namespace Yextly.OpenTelemetry;

/// <summary>
/// Mutable property bag used to enrich events and exceptions.
/// </summary>
public sealed class OtPropertyBag : ITelemetryPropertyBag
{
    /// <summary>
    /// Gets the inner property dictionary.
    /// </summary>
    public Dictionary<string, string> Data { get; } = [];

    /// <inheritdoc />
    public void Add(string name, string value)
    {
        Data[name] = value;
    }
}
