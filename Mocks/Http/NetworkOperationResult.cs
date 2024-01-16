// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Net;

namespace Yextly.Testing.Mocks.Http
{
    internal sealed class NetworkOperationResult
    {
        public NetworkOperationResult(HttpStatusCode statusCode, HttpContent content)
        {
            ArgumentNullException.ThrowIfNull(content);

            StatusCode = statusCode;
            Content = content;
        }

        public HttpContent Content { get; }
        public HttpStatusCode StatusCode { get; }
    }
}
