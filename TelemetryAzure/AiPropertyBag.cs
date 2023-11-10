// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Yextly.Telemetry.Abstractions;

namespace Yextly.Telemetry.Azure
{
    internal sealed class AiPropertyBag : ITelemetryPropertyBag
    {
        public Dictionary<string, string> Data { get; } = new(StringComparer.Ordinal);

        public void Add(string name, string value)
        {
            ArgumentNullException.ThrowIfNull(name);

            Data.Add(name, value ?? string.Empty);
        }
    }
}
