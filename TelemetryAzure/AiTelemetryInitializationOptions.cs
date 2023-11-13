// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Collections.Immutable;

namespace Yextly.Telemetry.Azure
{
    /// <summary>
    /// Provides Application Insights initialization options.
    /// </summary>
    public sealed class AiTelemetryInitializationOptions
    {
        private readonly List<Type> _types = new();

        /// <summary>
        /// Specifies whether or not the engine should be enabled or not.
        /// </summary>
        /// <remarks>You may disable the engine, in which case the record part will not work, but your application can run.</remarks>
        public bool Enabled { get; set; }

        /// <summary>
        /// Retrieves the initializers.
        /// </summary>
        public ImmutableArray<Type> Initializers => _types.ToImmutableArray();

        /// <summary>
        /// Adds a new initializer.
        /// </summary>
        /// <typeparam name="T">The type of the initializer to add.</typeparam>
        public void AddInitializer<T>() where T : IAiTelemetryClientInitializer
        {
            _types.Add(typeof(T));
        }
    }
}
