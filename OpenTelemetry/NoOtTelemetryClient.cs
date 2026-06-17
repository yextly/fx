// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Yextly.Telemetry.Abstractions;

namespace Yextly.OpenTelemetry;

/// <summary>
/// No-op telemetry client.
/// </summary>
public sealed class NoOtTelemetryClient : NoOtTelemetryClientCore
{
}

/// <summary>
/// No-op telemetry client.
/// </summary>
public sealed class NoOtTelemetryClient<T> : NoOtTelemetryClientCore, ITelemetryClient<T> where T : ITelemetryActivitySourceProvider, new()
{
}
