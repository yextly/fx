// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Immutable;

namespace Yextly.Testing.Mocks.Http
{
    /// <summary>
    /// A factory abstraction for a component that can create <see cref="HttpClient"/> instances with custom
    /// configuration for a given logical name.
    /// </summary>
    /// <remarks>
    /// A default <see cref="IHttpClientFactory"/> can be registered in an <see cref="IServiceCollection"/>
    /// by calling <see cref="HttpClientFactoryServiceCollectionExtensions.AddHttpClient(IServiceCollection)"/>.
    /// The default <see cref="IHttpClientFactory"/> will be registered in the service collection as a singleton.
    /// </remarks>
    public sealed class MockedHttpClientFactory : IMockedHttpClientFactory, IHttpClientFactory, IDisposable
    {
        internal static readonly string DefaultHandlerName = typeof(HttpClient).Name;

        private readonly HttpMessageHandler _defaultHandler;
        private readonly ImmutableDictionary<string, HttpMessageHandler> _handlers;
        private readonly IOptionsMonitor<HttpClientFactoryOptions> _optionsMonitor;

        /// <summary>
        /// Creates a new <see cref="MockedHttpClientFactory"/> from a dependency injection (DI) container.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="optionsMonitor"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="options"></param>
        public MockedHttpClientFactory(ILogger<MockedHttpClientFactory> logger, IOptionsMonitor<HttpClientFactoryOptions> optionsMonitor, IServiceProvider serviceProvider, IEnumerable<IMockedHttpClientOptions> options) : this(logger, optionsMonitor, serviceProvider, Convert(options))
        {
        }

        private MockedHttpClientFactory(ILogger logger, IOptionsMonitor<HttpClientFactoryOptions> optionsMonitor, IServiceProvider serviceProvider, IEnumerable<MockedHttpClientBuilderBinding> builders)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(optionsMonitor);
            ArgumentNullException.ThrowIfNull(serviceProvider);
            ArgumentNullException.ThrowIfNull(builders);

            _optionsMonitor = optionsMonitor;

            var finalHandlers = ImmutableDictionary.CreateBuilder<string, HttpMessageHandler>(StringComparer.Ordinal);

            foreach (var item in builders)
            {
                var name = item.Name;
                var innerHandler = item.Builder.CreateHandler(logger);
                var finalHandler = CreateFinalHandler(serviceProvider, name, innerHandler);

                finalHandlers.TryAdd(name, finalHandler);
            }

            _handlers = finalHandlers.ToImmutable();

            if (finalHandlers.TryGetValue(DefaultHandlerName, out var defaultHandler))
                _defaultHandler = defaultHandler;
            else
                _defaultHandler = new MockedHttpClientBuilder().CreateHandler(logger);
        }

        /// <summary>
        /// Manually constructs a new <see cref="MockedHttpClientFactory"/> instance.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="defaultClientBuilder"></param>
        /// <param name="optionsMonitor"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="builders"></param>
        /// <returns></returns>
        public static MockedHttpClientFactory Create(ILogger logger, MockedHttpClientBuilder defaultClientBuilder, IOptionsMonitor<HttpClientFactoryOptions> optionsMonitor, IServiceProvider serviceProvider, params MockedHttpClientBuilderBinding[] builders)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(defaultClientBuilder);
            ArgumentNullException.ThrowIfNull(optionsMonitor);
            ArgumentNullException.ThrowIfNull(serviceProvider);
            ArgumentNullException.ThrowIfNull(builders);

            var finalBuilders = builders
                .Concat(Enumerable.Repeat(new MockedHttpClientBuilderBinding(DefaultHandlerName, defaultClientBuilder), 1));

            return new MockedHttpClientFactory(logger, optionsMonitor, serviceProvider, finalBuilders);
        }

        /// <inheritdoc/>
        public HttpClient CreateClient(string name)
        {
            HttpMessageHandler handler;

            if (string.IsNullOrWhiteSpace(name))
            {
                handler = _defaultHandler;
            }
            else
            {
                handler = _handlers.TryGetValue(name, out var t) ? t : _defaultHandler;
            }

            var client = new HttpClient(handler, disposeHandler: false);

            var options = _optionsMonitor.Get(name);

            var actions = options.HttpClientActions;
            for (int i = 0; i < actions.Count; i++)
            {
                actions[i](client);
            }

            return client;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            using (_defaultHandler)
            {
                foreach (var item in _handlers)
                {
                    using (item.Value)
                    {
                    }
                }
            }
        }

        private static IEnumerable<MockedHttpClientBuilderBinding> Convert(IEnumerable<IMockedHttpClientOptions> options)
        {
            return options.Select(x => new MockedHttpClientBuilderBinding(x.Name, x.Builder));
        }

        private HttpMessageHandler CreateFinalHandler(IServiceProvider serviceProvider, string name, HttpMessageHandler innerHandler)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(innerHandler);

            HttpClientFactoryOptions options = _optionsMonitor.Get(name);

            var builder = new MockedHttpMessageHandlerBuilder(name, innerHandler, serviceProvider);

            for (int i = 0; i < options.HttpMessageHandlerBuilderActions.Count; i++)
            {
                options.HttpMessageHandlerBuilderActions[i](builder);
            }

            return builder.Build();
        }
    }
}
