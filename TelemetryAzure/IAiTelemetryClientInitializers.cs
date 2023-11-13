// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Collections.Immutable;

namespace Yextly.Telemetry.Azure
{
    /// <summary>
    /// Provides external plugins to register and configure the Application Insight telemetry client.
    /// </summary>
    public interface IAiTelemetryClientInitializers
    {
        /// <summary>
        /// Returns the list of all the initializers registered.
        /// </summary>
        ImmutableArray<Type> Initializers { get; }
    }
}
