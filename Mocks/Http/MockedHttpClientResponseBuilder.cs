// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Net;
using Yextly.Testing.Mocks.Http;

namespace Yextly.Testing.Mocks.Http
{
    internal sealed class MockedHttpClientResponseBuilder : IMockedHttpClientResponseBuilder
    {
        private readonly Chain _chain;
        private readonly HttpMethod _method;
        private readonly MockedHttpClientBuilder _owner;
        private readonly Uri _uri;

        public MockedHttpClientResponseBuilder(MockedHttpClientBuilder owner, Chain chain, HttpMethod method, Uri uri)
        {
            ArgumentNullException.ThrowIfNull(owner);
            ArgumentNullException.ThrowIfNull(chain);
            ArgumentNullException.ThrowIfNull(method);
            ArgumentNullException.ThrowIfNull(uri);

            _owner = owner;
            _chain = chain;
            _method = method;
            _uri = uri;
        }

        public IMockedHttpClientBuilder Reply(HttpContent content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            ArgumentNullException.ThrowIfNull(content);

            var operation = new OperationFlow(_method, _uri, statusCode, content, null);
            _chain.Enqueue(operation);

            return _owner;
        }

        public IMockedHttpClientBuilder Reply(Action<HttpRequestMessage, HttpResponseMessage> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            var operation = new OperationFlow(_method, _uri, HttpStatusCode.OK, null, action);
            _chain.Enqueue(operation);

            return _owner;
        }
    }
}
