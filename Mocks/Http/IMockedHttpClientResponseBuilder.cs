// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Net;

namespace Yextly.Testing.Mocks.Http
{
    /// <summary>
    /// Provides the APIs to build a mock for <see cref="HttpClient"/>.
    /// </summary>
    public interface IMockedHttpClientResponseBuilder
    {
        /// <summary>
        /// Replies with static content.
        /// </summary>
        /// <param name="content">The content to reply.</param>
        /// <param name="statusCode">The status code to reply.</param>
        /// <returns></returns>
        IMockedHttpClientBuilder Reply(HttpContent content, HttpStatusCode statusCode = HttpStatusCode.OK);

        /// <summary>
        /// Replies with an action.
        /// </summary>
        /// <param name="action">The action used to fill the response.</param>
        /// <returns></returns>
        IMockedHttpClientBuilder Reply(Action<HttpRequestMessage, HttpResponseMessage> action);
    }
}
