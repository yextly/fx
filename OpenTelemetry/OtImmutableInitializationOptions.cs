// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Diagnostics;

namespace Yextly.OpenTelemetry
{
    /// <summary>
    /// Initialization options for OpenTelemetry-based telemetry support.
    /// </summary>
    public sealed record OtImmutableInitializationOptions
    {
        /// <summary>
        /// Gets or sets a value containing the root activity source.
        /// </summary>
        public required ActivitySource ActivitySource { get; init; }

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
