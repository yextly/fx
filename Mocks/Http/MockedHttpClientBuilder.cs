// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.Logging;
using Yextly.Common;

namespace Yextly.Testing.Mocks.Http
{
    /// <summary>
    /// Provides a builder that allows to mock and provided expectations about an <see cref="HttpClient"/>.
    /// </summary>
    public sealed partial class MockedHttpClientBuilder : IMockedHttpClientBuilder
    {
        private readonly OperationChain _chain = new();
        private readonly MutableMockedHttpClientOptions _options;

        /// <summary>
        /// Creates a new <see cref="MockedHttpClientBuilder"/> instance.
        /// </summary>
        public MockedHttpClientBuilder()
        {
            _options = new()
            {
                DefaultDelay = TimeSpan.FromMilliseconds(100),
                DefaultAsynchronousDelay = TimeSpan.FromMilliseconds(100),
            };
        }

        /// <summary>
        /// Builds a new <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="logger">The logging instance to use.</param>
        /// <param name="lifetimeContainer">A container used for tracking and collecting all long-lived instances that need to be disposed.</param>
        /// <returns></returns>
        public HttpClient Build(ILogger logger, IDisposableProducerConsumerCollection<IDisposable> lifetimeContainer)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(lifetimeContainer);

            var options = CaptureOptions();

            var handler = new MockedHttpMessageHandler(logger, options);

            // If the item cannot be added, we could be under memory pressure. What do we need to do here?
            lifetimeContainer.TryAdd(handler);

            return new HttpClient(handler, disposeHandler: false);
        }

        /// <inheritdoc/>
        public IMockedHttpClientBuilder Configure(Action<IMockedHttpClientOptions> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);

            configure(_options);

            return this;
        }

        /// <inheritdoc/>
        public IMockedHttpClientResponseBuilder Expect(HttpMethodOperation method, Uri uri)
        {
            ArgumentNullException.ThrowIfNull(uri);

            var m = method switch
            {
                HttpMethodOperation.Get => HttpMethod.Get,
                HttpMethodOperation.Post => HttpMethod.Post,
                HttpMethodOperation.Put => HttpMethod.Put,
                HttpMethodOperation.Delete => HttpMethod.Delete,
                HttpMethodOperation.Patch => HttpMethod.Patch,
                HttpMethodOperation.Connect => HttpMethod.Connect,
                HttpMethodOperation.Head => HttpMethod.Head,
                HttpMethodOperation.Options => HttpMethod.Options,
                HttpMethodOperation.Trace => HttpMethod.Trace,
                _ => throw new NotSupportedException($@"The value ""{(int)method}"" is not supported."),
            };

            return new MockedHttpClientResponseBuilder(this, _chain, m, uri);
        }

        internal HttpMessageHandler CreateHandler(ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            var options = CaptureOptions();

            var handler = new MockedHttpMessageHandler(logger, options);

            return handler;
        }

        private MockedHttpClientOptions CaptureOptions()
        {
            var chain = _chain.Clone();

            var options = new MockedHttpClientOptions
            {
                Chain = chain,
                DefaultAsynchronousDelay = _options.DefaultAsynchronousDelay,
                DefaultSynchronousDelay = _options.DefaultDelay,
            };
            return options;
        }
    }
}
