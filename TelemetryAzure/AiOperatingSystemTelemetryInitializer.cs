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
