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
    public sealed class AiTelemetryClientInitializers : IAiTelemetryClientInitializers
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="initializer"></param>
        public AiTelemetryClientInitializers(ImmutableArray<Type> initializer)
        {
            Initializers = initializer;
        }

        /// <summary>
        /// Returns the registered initializers.
        /// </summary>
        public ImmutableArray<Type> Initializers { get; }
    }
}
