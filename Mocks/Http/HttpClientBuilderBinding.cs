// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Testing.Mocks.Http
{
    public readonly record struct HttpClientBuilderBinding
    {
        public HttpClientBuilderBinding(string name, HttpClientMockBuilder builder)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(builder);

            Name = name;
            Builder = builder;
        }

        public string Name { get; }
        public HttpClientMockBuilder Builder { get; }
    }
}
