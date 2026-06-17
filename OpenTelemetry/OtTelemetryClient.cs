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
public sealed class OtTelemetryClient : OtTelemetryClientCore
{
    /// <inheritdoc />
    public OtTelemetryClient(OtImmutableInitializationOptions options, IServiceProvider serviceProvider) : base(options, serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(serviceProvider);
    }
}

/// <summary>
/// <see cref="ITelemetryClient{T}" /> implementation backed by <see cref="ActivitySource" />.
/// </summary>
public sealed class OtTelemetryClient<T> : OtTelemetryClientCore, ITelemetryClient<T> where T : ITelemetryActivitySourceProvider, new()
{
    /// <inheritdoc />
    public OtTelemetryClient(OtImmutableInitializationOptions options, IServiceProvider serviceProvider) : base(CreateOptions(options), serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(serviceProvider);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "We can't crash in the DI constructor")]
    private static OtImmutableInitializationOptions CreateOptions(OtImmutableInitializationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        try
        {
            var provider = new T();

            // Here we do not handle IsEnabled on purpose: the DI registrar will register a no-op when disabled.
            var sourceName = provider.SourceName ?? options.ActivitySourceName;
            var source = new ActivitySource(sourceName);

            var newOptions = options with
            {
                ActivitySourceName = sourceName,
                ActivitySource = source,
            };

            return newOptions;
        }
        catch
        {
            // In case of bad things, we don't want to smash the whole application with an exception hard to track.
            return options;
        }
    }
}
