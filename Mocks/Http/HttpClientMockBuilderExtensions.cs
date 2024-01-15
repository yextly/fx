// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.Logging;

namespace Yextly.Testing.Mocks.Http
{
    /// <summary>
    /// Provides extension methods to create <see cref="HttpClient"/> instances.
    /// </summary>
    public static class HttpClientMockBuilderExtensions
    {
        private static readonly Lazy<LeakingContainer> _leakingContainer = new(() => new LeakingContainer(), LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Builds a new <see cref="HttpClient"/>.
        /// </summary>
        /// <returns></returns>
        public static HttpClient Build(this HttpClientMockBuilder instance)
        {
            ArgumentNullException.ThrowIfNull(instance);

            // NullLogger can be disposed multiple times safely since it does nothing
            using var logger = new NullLogger();

            return instance.Build(logger, _leakingContainer.Value);
        }

        /// <summary>
        /// Builds a new <see cref="HttpClient"/>.
        /// </summary>
        /// <returns></returns>
        public static HttpClient Build(this HttpClientMockBuilder instance, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(instance);
            ArgumentNullException.ThrowIfNull(logger);

            return instance.Build(logger, _leakingContainer.Value);
        }
    }
}
