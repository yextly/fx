// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Yextly.Common;
using Yextly.Testing.Mocks.Http;

namespace Yextly.Testing.Mocks.Http
{
    /// <summary>
    /// Provides a builder that allows to mock and provided expectations about an <see cref="HttpClient"/>.
    /// </summary>
    public sealed partial class HttpClientMockBuilder : IHttpClientMockBuilder
    {
        private readonly Chain _chain = new();
        private readonly Delays _delays;

        /// <summary>
        /// Creates a new <see cref="HttpClientMockBuilder"/> instance.
        /// </summary>
        public HttpClientMockBuilder()
        {
            _delays = new Delays
            {
                SyncReplyDelay = TimeSpan.FromMilliseconds(100),
                AsyncReplyDelay = TimeSpan.FromMilliseconds(100),
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

            var handler = new HttpMessageHandlerMock(logger, _delays, _chain.Clone());

            // If the item cannot be added, we could under memory pressure. What do we need to do here?
            lifetimeContainer.TryAdd(handler);

            return new HttpClient(handler, disposeHandler: false);
        }

        /// <inheritdoc/>
        public IHttpClientMockResponseBuilder Expect(HttpMethodOperation method, Uri uri)
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

            return new HttpClientMockResponseBuilder(this, _chain, m, uri);
        }

        internal HttpMessageHandler CreateHandler(ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            var handler = new HttpMessageHandlerMock(logger, _delays, _chain.Clone());

            return handler;
        }
    }
}
