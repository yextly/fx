// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Diagnostics;
using Yextly.Telemetry.Abstractions;

namespace Yextly.OpenTelemetry;

/// <summary>
/// <see cref="ITelemetryClient" /> implementation backed by <see cref="ActivitySource" />.
/// </summary>
public class OtTelemetryClientCore : ITelemetryClient
{
    private readonly OtImmutableInitializationOptions _options;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="OtTelemetryClient" /> class.
    /// </summary>
    public OtTelemetryClientCore(OtImmutableInitializationOptions options, IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        ArgumentNullException.ThrowIfNull(options.ActivitySourceName);
        ArgumentNullException.ThrowIfNull(options.ActivitySource);

        // Forces the creation of the Otel pipeline
        _ = serviceProvider.GetService<TracerProvider>();
        _ = serviceProvider.GetService<MeterProvider>();

        _options = options;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets the activity source used by this telemetry client.
    /// </summary>
    public ActivitySource ActivitySource => _options.ActivitySource;

    /// <inheritdoc />
    public ITelemetryPropertyBag CreatePropertyBag()
    {
        return new OtPropertyBag();
    }

    /// <inheritdoc />
    public async Task<bool> FlushAsync(CancellationToken cancellationToken = default)
    {
        await Task.Yield();

        var provider = _serviceProvider.GetService<TracerProvider>();

        provider?.ForceFlush();

        return true;
    }

    /// <inheritdoc />
    public void TrackEvent(string eventName, ITelemetryPropertyBag? properties = default)
    {
        Activity? activity = Activity.Current;

        if (activity is null)
        {
            using var operation = (OtOperation)TrackOperation(eventName, "event");
            activity = Activity.Current;
        }

        if (activity is not null)
        {
            TrackEventCore(activity, eventName, properties);
        }
    }

    /// <inheritdoc />
    public void TrackException(Exception exception, ITelemetryPropertyBag? properties = default)
    {
        ArgumentNullException.ThrowIfNull(exception);

        Activity? activity = Activity.Current;

        if (activity is null)
        {
            using var operation = (OtOperation)TrackOperation(exception.GetType().Name, "exception");
            activity = Activity.Current;
        }

        if (activity is not null)
        {
            TrackExceptionCore(activity, exception, properties);
        }
    }

    /// <inheritdoc />
    public ITelemetryOperation TrackOperation(string operationName, string type)
    {
        return TrackOperation(operationName, type, null, null);
    }

    /// <inheritdoc />
    public ITelemetryOperation TrackOperation(string operationName, string type, string? operationId, string? parentOperationId = default)
    {
        var kind = MapActivityKind(type);

        return TrackOperation(operationName, type, kind, operationId, parentOperationId);
    }

    /// <summary>
    /// Tracks an operation and returns an instance implementing <see cref="ITelemetryOperation" /> which must be kept alive for the whole duration of the operation that is being tracked.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    /// <param name="type">The type of the operation to track.</param>
    /// <remarks>The meaning of <paramref name="type" /> depends on the precise implementation used.</remarks>
    /// <param name="kind">The kind of the operation to track. This is used to determine the appropriate ActivityKind for the underlying Activity.</param>
    /// <param name="operationId">The already computed operation id (usually used for integrations).</param>
    /// <param name="parentOperationId">The known parent id of the activity (usually used for integrations).</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Many things could go wrong, and we ignore which exception types could be thrown.")]
    public ITelemetryOperation TrackOperation(string operationName, string type, ActivityKind kind, string? operationId, string? parentOperationId = default)
    {
        ActivityContext? parentContext = null;

        // Convert legacy Application Insights identifiers -> OpenTelemetry context
        if (!string.IsNullOrWhiteSpace(operationId))
        {
            try
            {
                var traceId = ActivityTraceId.CreateFromString(operationId.AsSpan());

                var spanId = !string.IsNullOrWhiteSpace(parentOperationId)
                    ? ActivitySpanId.CreateFromString(parentOperationId.AsSpan())
                    : ActivitySpanId.CreateRandom();

                parentContext = new ActivityContext(traceId, spanId, ActivityTraceFlags.Recorded);
            }
            catch
            {
                // Ignore invalid IDs (avoid crashing the telemetry pipeline)
                parentContext = null;
            }
        }

        var activity = StartActivity(operationName, kind, parentContext, null);

        if (activity != null && !string.IsNullOrWhiteSpace(type))
        {
            activity.SetTag("dependency.type", type);
        }

        return new OtOperation(this, activity);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Required internally non static by design.")]
    internal void TrackEventCore(Activity activity, string eventName, ITelemetryPropertyBag? properties)
    {
        ArgumentNullException.ThrowIfNull(activity);

        activity.AddEvent(new ActivityEvent(eventName, tags: CreateTags(properties)));
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Required internally non static by design.")]
    internal void TrackExceptionCore(Activity activity, Exception exception, ITelemetryPropertyBag? properties)
    {
        ArgumentNullException.ThrowIfNull(activity);
        ArgumentNullException.ThrowIfNull(exception);

        activity.SetStatus(ActivityStatusCode.Error, exception.Message);

        var tags = new ActivityTagsCollection
        {
            { "exception.type", exception.GetType().FullName },
            { "exception.message", exception.Message },
            { "exception.stacktrace", exception.ToString() },
        };

        if (properties is OtPropertyBag propertyBag)
        {
            foreach ((string key, string value) in propertyBag.Data)
            {
                tags[key] = value;
            }
        }

        activity.AddEvent(new ActivityEvent("exception", tags: tags));
    }

    private static ActivityTagsCollection? CreateTags(ITelemetryPropertyBag? properties)
    {
        if (properties is not OtPropertyBag propertyBag || propertyBag.Data.Count == 0)
        {
            return null;
        }

        var tags = new ActivityTagsCollection();

        foreach ((string key, string value) in propertyBag.Data)
        {
            tags[key] = value;
        }

        return tags;
    }

    private static ActivityKind MapActivityKind(string type)
    {
        return type switch
        {
            "http" => ActivityKind.Client,
            "db" => ActivityKind.Client,
            "messaging" => ActivityKind.Producer,
            "server" => ActivityKind.Server,
            _ => ActivityKind.Internal
        };
    }

    private Activity? StartActivity(string operationName, ActivityKind kind, ActivityContext? parentContext, IEnumerable<KeyValuePair<string, object?>>? tags)
    {
        var activity = parentContext is null
            ? ActivitySource.StartActivity(operationName, kind, parentId: null, tags: tags)
            : ActivitySource.StartActivity(operationName, kind, parentContext.Value, tags);

        if (activity is null)
        {
            activity = new Activity(operationName);

            if (parentContext is not null)
            {
                activity.SetParentId(parentContext.Value.TraceId, parentContext.Value.SpanId, parentContext.Value.TraceFlags);
            }

            activity.Start();
        }

        return activity;
    }
}