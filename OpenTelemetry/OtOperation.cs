// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Diagnostics;
using Yextly.Telemetry.Abstractions;

namespace Yextly.OpenTelemetry;

/// <summary>
/// <see cref="ITelemetryOperation" /> implementation backed by <see cref="Activity" />.
/// </summary>
public sealed class OtOperation : ITelemetryOperation
{
    private readonly Activity? _activity;
    private readonly OtTelemetryClientCore _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="OtOperation" /> class.
    /// </summary>
    /// <param name="client">Owning telemetry client.</param>
    /// <param name="activity">Activity that represents the operation.</param>
    public OtOperation(OtTelemetryClientCore client, Activity? activity)
    {
        ArgumentNullException.ThrowIfNull(client);

        _client = client;
        _activity = activity;
    }

    /// <inheritdoc />
    public string Id => _activity?.Id ?? string.Empty;

    /// <inheritdoc />
    public void AddBaggage(string name, string? value)
    {
        _activity?.AddBaggage(name, value);
    }

    /// <inheritdoc />
    public void AddTag(string name, string value)
    {
        _activity?.SetTag(name, value);
    }

    /// <inheritdoc />
    public ITelemetryPropertyBag CreatePropertyBag()
    {
        return _client.CreatePropertyBag();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _activity?.Stop();
    }

    /// <inheritdoc />
    public void TrackEvent(string eventName, ITelemetryPropertyBag? properties = default)
    {
        if (_activity is not null)
        {
            _client.TrackEventCore(_activity, eventName, properties);
            return;
        }

        _client.TrackEvent(eventName, properties);
    }

    /// <inheritdoc />
    public void TrackException(Exception exception, ITelemetryPropertyBag? properties = default)
    {
        ArgumentNullException.ThrowIfNull(exception);

        if (_activity is not null)
        {
            _client.TrackExceptionCore(_activity, exception, properties);
            return;
        }

        _client.TrackException(exception, properties);
    }
}
