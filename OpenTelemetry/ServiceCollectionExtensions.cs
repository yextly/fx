using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics;
using Yextly.Telemetry.Abstractions;

using Yextly.Telemetry.OpenTelemetry;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Dependency injection registrations for Yextly OpenTelemetry support.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the OpenTelemetry-based telemetry abstractions and publishes the ActivitySource to the OpenTelemetry pipeline.
    /// </summary>
    /// <param name="services">The service collection to update.</param>
    /// <param name="configure">Optional configuration callback.</param>
    /// <returns>The updated service collection.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Technically this is correct since we are in the root.")]
    public static IServiceCollection AddOpenTelemetryHelpers(this IServiceCollection services, Action<OtInitializationOptions>? configure = null)
    {
        var options = new OtInitializationOptions();
        configure?.Invoke(options);

        var sourceName = options.DefaultActivitySourceName ?? "Yextly.Telemetry.OpenTelemetry";
        var source = new ActivitySource(sourceName);

        var snapshot = new OtImmutableInitializationOptions
        {
            ActivitySourceName = sourceName,
            Enabled = options.Enabled,
            ActivitySource = source,
            ActivitySourcePattern = options.ActivitySourcePattern,
        };

        services.TryAddSingleton(snapshot);

        services.AddOpenTelemetry()
            .WithTracing(builder => builder.AddSource(snapshot.ActivitySourceName));

        if (!string.IsNullOrWhiteSpace(snapshot.ActivitySourcePattern))
        {
            services.AddOpenTelemetry()
                .WithTracing(builder => builder.AddSource(snapshot.ActivitySourcePattern));
        }

        if (snapshot.Enabled)
        {
            services.TryAddSingleton<ITelemetryClient, OtTelemetryClient>();
            services.TryAddSingleton(typeof(ITelemetryClient<>), typeof(OtTelemetryClient<>));
        }
        else
        {
            services.TryAddSingleton<ITelemetryClient, NoOtTelemetryClient>();
            services.TryAddSingleton(typeof(ITelemetryClient<>), typeof(NoOtTelemetryClient<>));
        }

        services.TryAddSingleton<ITelemetryClientAccessor, OtTelemetryClientAccessor>();

        return services;
    }
}
