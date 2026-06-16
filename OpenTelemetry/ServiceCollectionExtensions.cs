using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics;
using Yextly.OpenTelemetry;

using Yextly.Telemetry.Abstractions;

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
    public static IServiceCollection AddOpenTelemetryAzureTelemetry(this IServiceCollection services, Action<OtInitializationOptions>? configure = null)
    {
        var options = new OtInitializationOptions();
        configure?.Invoke(options);
        var snapshot = new OtImmutableInitializationOptions
        {
            ActivitySourceName = options.ActivitySourceName ?? "Yextly.OpenTelemetry",
            Enabled = options.Enabled
        };
        options.Clone();

        services.TryAddSingleton(snapshot);
        services.AddSingleton(new ActivitySource(snapshot.ActivitySourceName));

        services.AddOpenTelemetry()
            .WithTracing(builder => builder.AddSource(snapshot.ActivitySourceName));

        if (snapshot.Enabled)
        {
            services.TryAddSingleton<ITelemetryClient, OtTelemetryClient>();
        }
        else
        {
            services.TryAddSingleton<ITelemetryClient, NoOtTelemetryClient>();
        }

        return services;
    }
}
