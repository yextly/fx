// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Testing.Mocks.Http
{
    public interface IMockedHttpClientOptions
    {
        HttpClientMockBuilder Builder { get; }
        string Name { get; }
    }
}
