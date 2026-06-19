// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Telemetry.Abstractions
{
    /// <summary>
    /// Represents the abstract telemetry client.
    /// </summary>
    public interface ITelemetryClientAccessor
    {
        /// <summary>
        /// Returns a telemetry client for the specified type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of the provider.</typeparam>
        /// <returns></returns>
        ITelemetryClient<T> GetClient<T>() where T : ITelemetryActivitySourceProvider, new();
    }
}
