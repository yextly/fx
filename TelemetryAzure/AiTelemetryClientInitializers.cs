// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Collections.Immutable;

namespace Yextly.Telemetry.Azure
{
    /// <summary>
    /// Provides the initialization of the Application Insights Telemetry clients.
    /// </summary>
    /// <remarks>This API supports the telemetry infrastructure and is not intended to be used directly from your code.</remarks>
    /// <remarks>
    /// Creates a new instance.
    /// </remarks>
    /// <param name="initializer"></param>
    public sealed class AiTelemetryClientInitializers(ImmutableArray<Type> initializer) : IAiTelemetryClientInitializers
    {
        /// <summary>
        /// Returns the registered initializers.
        /// </summary>
        public ImmutableArray<Type> Initializers { get; } = initializer;
    }
}
