// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Diagnostics;
using Yextly.Telemetry.Abstractions;

namespace Yextly.OpenTelemetry;

/// <summary>
/// <see cref="ITelemetryClient" /> implementation backed by <see cref="ActivitySource" />.
/// </summary>
public sealed class OtTelemetryClient : ITelemetryClient
{
    private const ActivityKind DefaultKind = ActivityKind.Internal;

    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="OtTelemetryClient" /> class.
    /// </summary>
    /// <param name="activitySource">The source used to create telemetry operations.</param>
    /// <param name="serviceProvider">Service provider used to resolve enrichers.</param>
    public OtTelemetryClient(ActivitySource activitySource, IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(activitySource);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        ActivitySource = activitySource;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets the activity source used by this telemetry client.
    /// </summary>
    public ActivitySource ActivitySource { get; }

    /// <inheritdoc />
    public ITelemetryPropertyBag CreatePropertyBag()
    {
        return new OtPropertyBag();
    }

    /// <inheritdoc />
    public Task<bool> FlushAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public void TrackEvent(string eventName, ITelemetryPropertyBag? properties = default)
    {
        Activity? activity = Activity.Current;
        if (activity is null)
        {
            using OtOperation operation = (OtOperation)TrackOperation(eventName, "event");
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
            using OtOperation operation = (OtOperation)TrackOperation(exception.GetType().Name, "exception");
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
        return TrackOperation(operationName, type, string.Empty, null);
    }

    /// <inheritdoc />
    public ITelemetryOperation TrackOperation(string operationName, string type, string operationId, string? parentOperationId = default)
    {
        string? parentId;

        if (!string.IsNullOrWhiteSpace(parentOperationId))
        {
            parentId = parentOperationId;
        }
        else
        {
            parentId = (!string.IsNullOrWhiteSpace(operationId) ? operationId : null);
        }

        var tags = new List<KeyValuePair<string, object?>>
        {
            new("yextly.telemetry.dependency_type", type),
        };

        if (!string.IsNullOrWhiteSpace(operationId))
        {
            tags.Add(new("yextly.telemetry.operation_id", operationId));
        }

        if (!string.IsNullOrWhiteSpace(parentOperationId))
        {
            tags.Add(new("yextly.telemetry.parent_operation_id", parentOperationId));
        }

        Activity? activity = StartActivity(operationName, parentId, tags);
        return new OtOperation(this, activity);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Required internally non static by design.")]
    internal void TrackEventCore(Activity activity, string eventName, ITelemetryPropertyBag? properties)
    {
        activity.AddEvent(new ActivityEvent(eventName, tags: CreateTags(properties)));
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Required internally non static by design.")]
    internal void TrackExceptionCore(Activity activity, Exception exception, ITelemetryPropertyBag? properties)
    {
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

    private Activity? StartActivity(string operationName, string? parentId, IEnumerable<KeyValuePair<string, object?>>? tags)
    {
        Activity? activity = string.IsNullOrWhiteSpace(parentId)
            ? ActivitySource.StartActivity(DefaultKind, tags: tags, name: operationName)
            : ActivitySource.StartActivity(operationName, DefaultKind, parentId: parentId, tags: tags);

        if (activity is null)
        {
            activity = new Activity(operationName);
            if (!string.IsNullOrWhiteSpace(parentId))
            {
                activity.SetParentId(parentId);
            }

            if (tags is not null)
            {
                foreach ((string key, object? value) in tags)
                {
                    activity.SetTag(key, value);
                }
            }

            activity.Start();
        }

        return activity;
    }
}
