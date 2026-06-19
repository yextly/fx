// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using OpenTelemetry;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Yextly.Telemetry.OpenTelemetry;

/// <summary>
/// Adds current operating system information to newly created activities.
/// </summary>
public sealed class OperatingSystemActivityEnricher : BaseProcessor<Activity>
{
    /// <inheritdoc />
    public override void OnStart(Activity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);

        activity.SetTag("device.os.description", RuntimeInformation.OSDescription);
    }
}
