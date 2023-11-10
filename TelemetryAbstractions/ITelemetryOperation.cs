// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Telemetry.Abstractions
{
    /// <summary>
    /// Represents an operation that needs to be tracked.
    /// </summary>
    /// <remarks>The scope of the operation is determined by its creation and its explicit disposal via the Dispose method.</remarks>
    public interface ITelemetryOperation : ITelemetryCommonOperation, IDisposable
    {
        /// <summary>
        /// Provides the unique identifier of the current operation. This value is implementation dependent.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Adds a tag (metadata) to the current and future child operations.
        /// </summary>
        /// <param name="name">The metadata name.</param>
        /// <param name="value">The metadata value.</param>
        void AddBaggage(string name, string? value);

        /// <summary>
        /// Adds a tag (metadata) to the current operation.
        /// </summary>
        /// <param name="name">The metadata name.</param>
        /// <param name="value">The metadata value.</param>
        void AddTag(string name, string value);
    }
}
