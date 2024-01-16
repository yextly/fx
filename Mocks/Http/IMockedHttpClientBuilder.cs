// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Testing.Mocks.Http
{
    /// <summary>
    /// Provides common APIs used for building an <see cref="HttpClient"/> mock.
    /// </summary>
    public interface IMockedHttpClientBuilder
    {
        /// <summary>
        /// Creates a sequential expectation. The request is rejected if it does not match the provided <paramref name="method"/> and <paramref name="uri"/>.
        /// </summary>
        /// <param name="method">The HTTP method used for the request.</param>
        /// <param name="uri">The uri to match. The comparison is case sensitive and compares the whole uri, including segments and parts.</param>
        /// <returns></returns>
        IMockedHttpClientResponseBuilder Expect(HttpMethodOperation method, Uri uri);
    }
}
