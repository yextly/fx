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
public sealed class NoOtTelemetryClient : ITelemetryClient
{
    /// <inheritdoc />
    public ITelemetryPropertyBag CreatePropertyBag()
    {
        return new NullPropertyBag();
    }

    /// <inheritdoc />
    public Task<bool> FlushAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public void TrackEvent(string eventName, ITelemetryPropertyBag? properties = default)
    {
    }

    /// <inheritdoc />
    public void TrackException(Exception exception, ITelemetryPropertyBag? properties = default)
    {
    }

    /// <inheritdoc />
    public ITelemetryOperation TrackOperation(string operationName, string type)
    {
        return new NullOperation();
    }

    /// <inheritdoc />
    public ITelemetryOperation TrackOperation(string operationName, string type, string operationId, string? parentOperationId = null)
    {
        return new NullOperation();
    }

    private sealed class NullOperation : ITelemetryOperation
    {
        public string Id => string.Empty;

        public void AddBaggage(string name, string? value)
        {
        }

        public void AddTag(string name, string value)
        {
        }

        public ITelemetryPropertyBag CreatePropertyBag()
        {
            return new NullPropertyBag();
        }

        public void Dispose()
        {
        }

        public void TrackEvent(string eventName, ITelemetryPropertyBag? properties = default)
        {
        }

        public void TrackException(Exception exception, ITelemetryPropertyBag? properties = default)
        {
        }
    }

    private sealed class NullPropertyBag : ITelemetryPropertyBag
    {
        public void Add(string name, string value)
        {
        }
    }
}
