// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Immutable;

namespace Yextly.Testing.Mocks.Http
{
    public sealed class MockedHttpClientFactory : IMockedHttpClientFactory, IHttpClientFactory, IDisposable
    {
        internal static readonly string DefaultHandlerName = typeof(HttpClient).Name;

        private readonly HttpMessageHandler _defaultHandler;
        private readonly ImmutableDictionary<string, HttpMessageHandler> _handlers;
        private readonly IOptionsMonitor<HttpClientFactoryOptions> _optionsMonitor;

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
