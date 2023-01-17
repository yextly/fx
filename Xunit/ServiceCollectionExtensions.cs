// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Yextly.Xunit.Testing;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Contains Dependency Injection extensions.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds support for logging to XUnit..
        /// </summary>
        /// <param name="services">The service collection to use.</param>
        /// <param name="testOutputHelper">The destination of all the logs.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Arguments are null or empty.</exception>
        public static IServiceCollection AddXUnitLogging(this IServiceCollection services, ITestOutputHelper testOutputHelper)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(testOutputHelper);

            services.TryAddSingleton<ITestOutputHelper>(testOutputHelper);

            services.TryAddEnumerable(new ServiceDescriptor(typeof(ILogger<>), typeof(XUnitLogger<>), ServiceLifetime.Singleton));
            services.TryAddSingleton<ILoggerFactory, XUnitLoggerFactory>();

            return services;
        }
    }
}
