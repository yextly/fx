// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Telemetry.Abstractions
{
    /// <summary>
    /// Contains common methods.
    /// </summary>
    public interface ITelemetryCommonOperation
    {
        /// <summary>
        /// Creates a new property bag.
        /// </summary>
        /// <returns>A new property bag.</returns>
        /// <remarks>Property bags are not thread-safe and cannot be shared across calls, even from the same thread.</remarks>
        ITelemetryPropertyBag CreatePropertyBag();

        /// <summary>
        /// Tracks an application-based events.
        /// </summary>
        /// <param name="name">The name of the event to record.</param>
        /// <param name="tags">The additional tags (metadata) to relate to the event.</param>
        void TrackEvent(string name, ITelemetryPropertyBag? tags = null);

        /// <summary>
        /// Tracks an exception.
        /// </summary>
        /// <remarks>Exception data is automatically converted to telemetry data on a best-effort basis.</remarks>
        /// <param name="exception">The exception to record.</param>
        /// <param name="tags">The additional tags (metadata) to relate to the exception.</param>
        void TrackException(Exception exception, ITelemetryPropertyBag? tags = null);
    }
}
