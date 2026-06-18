using System;
using System.Diagnostics;
using Yextly.OpenTelemetry;

namespace Yextly.Telemetry.Abstractions
{
    /// <summary>
    /// Provides extensions for <see cref="ITelemetryClient" />.
    /// </summary>
    public static class TelemetryClientExtensions
    {
        /// <summary>
        /// Tracks an operation and returns an instance implementing <see cref="ITelemetryOperation" /> which must be kept alive for the whole duration of the operation that is being tracked.
        /// </summary>
        /// <param name="operationName">The name of the operation.</param>
        /// <param name="type">The type of the operation to track.</param>
        /// <remarks>The meaning of <paramref name="type" /> depends on the precise implementation used.</remarks>
        /// <param name="instance">The instance.</param>
        /// <param name="kind">The kind of the operation to track. This is used to determine the appropriate ActivityKind for the underlying Activity.</param>
        /// <returns></returns>
        public static ITelemetryOperation TrackOperation(this ITelemetryClient instance, string operationName, string type, ActivityKind kind)
        {
            ArgumentNullException.ThrowIfNull(instance);

            if (instance is OtTelemetryClientCore client)
            {
                return client.TrackOperation(operationName, type, kind, null, null);
            }
            else
            {
                return instance.TrackOperation(operationName, type);
            }
        }
    }
}
