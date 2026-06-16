using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics;
using Yextly.OpenTelemetry;

//using Yextly.OpenTelemetry.Compat;
using Yextly.Telemetry.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Dependency injection registrations for Yextly OpenTelemetry support.
/// </summary>
public static class ServiceCollectionExtensions
{
    ///// <summary>
    ///// Compatibility registration matching the legacy TelemetryAzure extension name.
    ///// </summary>
    ///// <param name="services">The service collection to update.</param>
    ///// <param name="configure">Optional compatibility configuration callback.</param>
    ///// <returns>The updated service collection.</returns>
    //[Obsolete("Use AddOpenTelemetryAzureTelemetry instead.")]
    //public static IServiceCollection AddApplicationInsightsTelemetryHelpers(this IServiceCollection services, Action<AiTelemetryInitializationOptions>? configure = null)
    //{
    //    var compatibilityOptions = new AiTelemetryInitializationOptions();
    //    configure?.Invoke(compatibilityOptions);

    //    return services.AddOpenTelemetryAzureTelemetry(options =>
    //    {
    //        options.Enabled = compatibilityOptions.Enabled;
    //        options.ActivitySourceName = compatibilityOptions.ActivitySourceName;

    //        foreach (Type type in compatibilityOptions.Initializers)
    //        {
    //            if (typeof(IOtActivityEnricher).IsAssignableFrom(type))
    //            {
    //                AddCompatibilityType(options, type);
    //            }
    //        }
    //    });
    //}

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
        var snapshot = options.Clone();

        services.TryAddSingleton(snapshot);
        services.AddSingleton(new ActivitySource(snapshot.ActivitySourceName));

        foreach (Type type in options.Enrichers)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IOtActivityEnricher), type));
        }

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

    //private static void AddCompatibilityType(OtInitializationOptions options, Type type)
    //{
    //    typeof(OtInitializationOptions)
    //        .GetMethod(nameof(OtInitializationOptions.AddEnricher))!
    //        .MakeGenericMethod(type)
    //        .Invoke(options, null);
    //}
}
