// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Testing.Mocks.Http
{
    internal sealed class MutableMockedHttpClientOptions : IMockedHttpClientOptions
    {
        public TimeSpan DefaultAsynchronousDelay { get; set; }
        public TimeSpan DefaultSynchronousDelay { get; set; }
    }
}
