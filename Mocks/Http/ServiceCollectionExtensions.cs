// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using System.Diagnostics.CodeAnalysis;
using Yextly.Testing.Mocks.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IMockedHttpClientFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a named <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        public static IHttpClientMockBuilder AddMockedHttpClient(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            return AddMockedHttpClient(services, MockedHttpClientFactory.DefaultHandlerName);
        }

        /// <summary>
        /// Adds the <see cref="IHttpClientFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a binding between the <typeparamref name="TClient"/> type and a named <see cref="HttpClient"/>. The client name
        /// will be set to the type name of <typeparamref name="TClient"/>.
        /// </summary>
        /// <typeparam name="TClient">
        /// The type of the typed client. The type specified will be registered in the service collection as
        /// a transient service. See <see cref="ITypedHttpClientFactory{TClient}" /> for more details about authoring typed clients.
        /// </typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
        /// <remarks>
        /// <para>
        /// <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using
        /// <see cref="IHttpClientFactory.CreateClient(string)"/> and providing the matching name.
        /// </para>
        /// <para>
        /// <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="HttpClient" />
        /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
        /// <typeparamref name="TClient"/> as the service type.
        /// </para>
        /// </remarks>
        public static IHttpClientMockBuilder AddMockedHttpClient<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TClient>(
            this IServiceCollection services)
            where TClient : class
        {
            ArgumentNullException.ThrowIfNull(services);

            return AddMockedHttpClient(services, typeof(TClient).Name);
        }

        /// <summary>
        /// Adds the <see cref="IMockedHttpClientFactory"/> and related services to the <see cref="IServiceCollection"/> and configures
        /// a named <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The logical name of the <see cref="HttpClient"/> to configure.</param>
        public static IHttpClientMockBuilder AddMockedHttpClient(this IServiceCollection services, string name)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);

            var builder = new HttpClientMockBuilder();

            var options = new MockedHttpClientOptions(name, builder);

            services.AddSingleton<IMockedHttpClientOptions, MockedHttpClientOptions>(_ => options);

            AddCommonServices(services);

            return builder;
        }

        private static void AddCommonServices(IServiceCollection services)
        {
            // here we register the same factory twice, this way you can override IHttpClientFactory and have a mean to access the mock when needed.
            services.TryAddSingleton<IMockedHttpClientFactory, MockedHttpClientFactory>();
            services.TryAddSingleton<IHttpClientFactory>(static services => services.GetRequiredService<IMockedHttpClientFactory>());
        }
    }
}
