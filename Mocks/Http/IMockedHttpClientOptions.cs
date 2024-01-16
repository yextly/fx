// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Testing.Mocks.Http
{
    /// <summary>
    /// Specifies options used to configure an <see cref="HttpClient"/> instance.
    /// </summary>
    public interface IMockedHttpClientOptions
    {
        /// <summary>
        /// The delay that must be introduced for every call.
        /// </summary>
        /// <remarks>The delay is always introduced before evaulating the expections.</remarks>
        TimeSpan DefaultDelay { get; set; }
    }
}
