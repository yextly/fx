// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Telemetry.Abstractions
{
    /// <summary>
    /// Represents a store of pairs (Key, Value) of metadata that can be attached to a telemetry event.
    /// </summary>
    public interface ITelemetryPropertyBag
    {
        /// <summary>
        /// Adds a new entry by name.
        /// </summary>
        /// <param name="name">The name of the entry.</param>
        /// <param name="value">The metadata value as string.</param>
        void Add(string name, string value);
    }
}
