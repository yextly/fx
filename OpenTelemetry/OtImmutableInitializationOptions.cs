// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.OpenTelemetry
{
    /// <summary>
    /// Initialization options for OpenTelemetry-based telemetry support.
    /// </summary>
    internal sealed record OtImmutableInitializationOptions
    {
        /// <summary>
        /// Gets or sets the activity source name used to create operations.
        /// </summary>
        public required string ActivitySourceName { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether telemetry is enabled.
        /// </summary>
        public bool Enabled { get; init; }
    }
}
