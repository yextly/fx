// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Testing.Mocks.Http
{
    internal sealed class MockedHttpClientDescriptor : IMockedHttpClientDescriptor
    {
        public MockedHttpClientDescriptor(string name, MockedHttpClientBuilder builder)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(builder);

            Name = name;
            Builder = builder;
        }

        public MockedHttpClientBuilder Builder { get; }
        public string Name { get; }
    }
}
