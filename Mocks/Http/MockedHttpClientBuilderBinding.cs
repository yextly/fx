// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Testing.Mocks.Http
{
    /// <summary>
    /// Represents a named binding between <see cref="MockedHttpClientBuilder"/> and <see cref="HttpClient"/>.
    /// </summary>
    public readonly record struct MockedHttpClientBuilderBinding
    {
        /// <summary>
        /// Constructs a new <see cref="MockedHttpClientBuilderBinding"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="HttpClient"/>.</param>
        /// <param name="builder">The builder.</param>
        public MockedHttpClientBuilderBinding(string name, MockedHttpClientBuilder builder)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(builder);

            Name = name;
            Builder = builder;
        }

        /// <summary>
        /// Returns the name of the <see cref="HttpClient"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Returns the builder from which construct a new <see cref="HttpClient"/>.
        /// </summary>
        public MockedHttpClientBuilder Builder { get; }
    }
}
