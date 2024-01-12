// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Net;

namespace Yextly.Testing.Mocks.Http
{
    public interface IHttpClientMockResponseBuilder
    {
        IHttpClientMockBuilder Reply(HttpContent content, HttpStatusCode statusCode = HttpStatusCode.OK);

        IHttpClientMockBuilder Reply(Action<HttpRequestMessage, HttpResponseMessage> action);
    }
}
