// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Telemetry.Abstractions
{
    /// <summary>
    /// Provides the necessary information to automatically create an activity source.
    /// </summary>
    public interface ITelemetryActivitySourceProvider
    {
        /// <summary>
        /// Returns a valid telemetry activity source name for the given type.
        /// </summary>
        /// <remarks>The returned value should follow reverse dns convention.</remarks>
        string SourceName { get; }
    }
}
