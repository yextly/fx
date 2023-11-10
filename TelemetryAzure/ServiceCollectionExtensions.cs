// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Immutable;
using Yextly.Telemetry.Abstractions;
using Yextly.Telemetry.Azure;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <inheritdoc />
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds support for Application Insights telemetry.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">The configure method.</param>
        /// <returns></returns>
        /// <remarks>You may need to add the proper Application Insights collectors before calling this method.</remarks>
        public static IServiceCollection AddApplicationInsightsTelemetry(this IServiceCollection services, Action<AiTelemetryInitializationOptions>? configure = null)
        {
            ArgumentNullException.ThrowIfNull(services);

            var options = new AiTelemetryInitializationOptions
            {
                Enabled = true,
            };

            configure?.Invoke(options);

            var initializers = options.Initializers
                .Where(x => x != null)
                .ToImmutableArray();

            services.TryAddSingleton<IAiTelemetryClientInitializers>(new AiTelemetryClientInitializers(initializers));

            if (options.Enabled)
                services.TryAddSingleton<ITelemetryClient, AiCoreTelemetryClient>();
            else
                services.TryAddSingleton<ITelemetryClient, AiNoTelemetryClient>();

            return services;
        }
    }
}
