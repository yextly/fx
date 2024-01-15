// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Net;

namespace Yextly.Testing.Mocks.Http
{
    internal sealed class OperationFlow
    {
        public OperationFlow(HttpMethod expectedMethod, Uri expectedUri, HttpStatusCode statusCode, HttpContent? content, Action<HttpRequestMessage, HttpResponseMessage>? action)
        {
            ArgumentNullException.ThrowIfNull(expectedMethod);
            ArgumentNullException.ThrowIfNull(expectedUri);

            ExpectedMethod = expectedMethod;
            ExpectedUri = expectedUri;
            StatusCode = statusCode;
            Content = content;
            Action = action;
        }

        public Action<HttpRequestMessage, HttpResponseMessage>? Action { get; }
        public HttpContent? Content { get; }
        public HttpMethod ExpectedMethod { get; }
        public Uri? ExpectedUri { get; }
        public HttpStatusCode StatusCode { get; }
    }
}
