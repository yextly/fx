// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Telemetry.OpenTelemetry;

/// <summary>
/// Initialization options for OpenTelemetry-based telemetry support.
/// </summary>
public sealed class OtInitializationOptions
{
    /// <summary>
    /// Gets or sets the pattern to used to register future activity sources.
    /// </summary>
    public string? ActivitySourcePattern { get; set; }

    /// <summary>
    /// Gets or sets the activity source name used to create operations when the non-generic client is used.
    /// </summary>
    public string? DefaultActivitySourceName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether telemetry is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;
}
