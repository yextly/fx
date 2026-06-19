// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Yextly.Telemetry.Abstractions;

namespace Yextly.Telemetry.OpenTelemetry;

/// <summary>
/// No-op telemetry client.
/// </summary>
public class NoOtTelemetryClientCore : ITelemetryClient
{
    private static readonly NullOperation _operation = new();
    private static readonly NullPropertyBag _propertyBag = new();

    /// <inheritdoc />
    public ITelemetryPropertyBag CreatePropertyBag()
    {
        return _propertyBag;
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
        return  _operation;
    }

    /// <inheritdoc />
    public ITelemetryOperation TrackOperation(string operationName, string type, string operationId, string? parentOperationId = null)
    {
        return  _operation;
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
            return _propertyBag;
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
