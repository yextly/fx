// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.ApplicationInsights;
using System.Runtime.InteropServices;

namespace Yextly.Telemetry.Azure
{
    /// <summary>
    /// Provides Application Insights initialization support.
    /// </summary>
    /// <remarks>This API supports the telemetry infrastructure and is not intended to be used directly from your code.</remarks>
    public sealed class AiOperatingSystemTelemetryInitializer : IAiTelemetryClientInitializer
    {
        /// <inheritdoc />
        public void Initialize(TelemetryClient client)
        {
            ArgumentNullException.ThrowIfNull(client);

            var operatingSystem = RuntimeInformation.OSDescription;

            client.Context.Device.OperatingSystem = operatingSystem;
        }
    }
}
