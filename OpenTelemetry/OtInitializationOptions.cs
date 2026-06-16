// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.OpenTelemetry;

/// <summary>
/// Initialization options for OpenTelemetry-based telemetry support.
/// </summary>
public sealed class OtInitializationOptions
{
    /// <summary>
    /// Gets or sets the activity source name used to create operations.
    /// </summary>
    public string? ActivitySourceName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether telemetry is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;
}
