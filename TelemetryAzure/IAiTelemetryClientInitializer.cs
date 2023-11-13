// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.ApplicationInsights;

namespace Yextly.Telemetry.Azure
{
    /// <summary>
    /// Represents a telemetry client initializer.
    /// </summary>
    public interface IAiTelemetryClientInitializer
    {
        /// <summary>
        /// Invoked by the engine on a newly created <see cref="TelemetryClient"/>.
        /// </summary>
        /// <param name="client"></param>
        void Initialize(TelemetryClient client);
    }
}
