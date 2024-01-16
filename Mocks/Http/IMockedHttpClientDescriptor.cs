// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Testing.Mocks.Http
{
    /// <summary>
    /// Contains options for <see cref="MockedHttpClientBuilder"/>
    /// </summary>
    /// <remarks>This is for internal use only.</remarks>
    public interface IMockedHttpClientDescriptor
    {
        /// <summary>
        /// References the builder.
        /// </summary>
        MockedHttpClientBuilder Builder { get; }

        /// <summary>
        /// Contains the name of the matching <see cref="HttpClient"/>.
        /// </summary>
        string Name { get; }
    }
}
