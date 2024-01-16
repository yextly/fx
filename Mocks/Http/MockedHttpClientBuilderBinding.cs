// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Testing.Mocks.Http
{
    public readonly record struct MockedHttpClientBuilderBinding
    {
        public MockedHttpClientBuilderBinding(string name, MockedHttpClientBuilder builder)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(builder);

            Name = name;
            Builder = builder;
        }

        public string Name { get; }
        public MockedHttpClientBuilder Builder { get; }
    }
}
