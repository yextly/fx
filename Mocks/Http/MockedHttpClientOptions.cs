// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Testing.Mocks.Http
{
    internal sealed record MockedHttpClientOptions
    {
        public TimeSpan DefaultAsynchronousDelay { get; init; }
        public TimeSpan DefaultSynchronousDelay { get; init; }
        public required OperationChain Chain { get; init; }
    }
}
