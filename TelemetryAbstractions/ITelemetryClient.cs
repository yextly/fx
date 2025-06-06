// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Telemetry.Abstractions
{
    /// <summary>
    /// Represents the abstraction client.
    /// </summary>
    public interface ITelemetryClient : ITelemetryCommonOperation
    {
        /// <summary>
        /// Asynchronously flushes the telemetry buffers.
        /// </summary>
        /// <remarks>You need to manually flush the buffers when the application exits</remarks>
        /// <param name="cancellationToken">The token to monitor for cancellation requests The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns></returns>
        Task<bool> FlushAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Tracks an operation and returns an instance implementing <see cref="ITelemetryOperation"/> which must be kept alive for the whole duration of the operation that is being tracked.
        /// </summary>
        /// <param name="operationName">The name of the operation.</param>
        /// <param name="type">The type of the operation to track.</param>
        /// <remarks>The meaning of <paramref name="type"/> depends on the precise implementation used.</remarks>
        /// <returns></returns>
        ITelemetryOperation TrackOperation(string operationName, string type);
    }
}
