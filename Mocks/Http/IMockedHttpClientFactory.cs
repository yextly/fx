// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.DependencyInjection;

namespace Yextly.Testing.Mocks.Http
{
    /// <summary>
    /// A factory abstraction for a component that can create <see cref="HttpClient"/> instances with custom
    /// configuration and mocks for a given logical name.
    /// </summary>
    /// <remarks>
    /// A default <see cref="IHttpClientFactory"/> can be registered in an <see cref="IServiceCollection"/>
    /// by calling <see cref="HttpClientFactoryServiceCollectionExtensions.AddHttpClient(IServiceCollection)"/>.
    /// The default <see cref="IHttpClientFactory"/> will be registered in the service collection as a singleton.
    /// </remarks>
    public interface IMockedHttpClientFactory : IHttpClientFactory;
}
