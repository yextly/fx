// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Yextly.Common;
using Yextly.Tasks;
using Yextly.Testing.Mocks.Http;
using Yextly.Xunit.Testing;

namespace MocksUnitTests.Http
{
    public sealed class HttpClientBasicTests : IDisposable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "It's a test")]
        private const string TestUri1 = "https://www.website1.blackhole/test.php?q=123";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "It's a test")]
        private const string TestUri2 = "https://www.website1.blackhole/test.php?q=999";

        private readonly IDisposableProducerConsumerCollection<IDisposable> _garbage;
        private readonly XUnitLoggerFactory _loggerFactory;
        private readonly ITestContextAccessor _testContextAccessor;

        public HttpClientBasicTests(ITestOutputHelper testOutputHelper, ITestContextAccessor testContextAccessor)
        {
            _loggerFactory = new XUnitLoggerFactory(testOutputHelper);
            _garbage = new ConcurrentBag<IDisposable>().AsDisposableCollection();
            _testContextAccessor = testContextAccessor;
        }

        [Fact]
        public async Task CanAcceptAMatchingRequest()
        {
            const string expectedContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse quis blandit lectus, vel facilisis odio. Morbi gravida non elit ac dignissim. Nullam at massa metus. Aenean euismod ex vitae suscipit cursus. Suspendisse vitae efficitur risus. Ut non leo nulla. Phasellus odio velit, molestie non congue nec, ornare at arcu. Fusce in interdum lectus. Pellentesque pulvinar nunc sagittis nisl porttitor lacinia. Cras quam libero, consectetur sit amet volutpat sed, gravida at turpis. Vivamus at dapibus nisi, non sollicitudin risus.";

            var cancellationToken = _testContextAccessor.Current.CancellationToken;

            var builder = new MockedHttpClientBuilder();

            using var content = new StringContent(expectedContent);

            builder
                .Expect(HttpMethodOperation.Get, new Uri(TestUri1, UriKind.Absolute))
                .Reply(content, HttpStatusCode.OK);

            using var client = builder.Build(_loggerFactory.CreateLogger<HttpClientBasicTests>(), _garbage);

            var result = await client.GetAsync(new Uri(TestUri1, UriKind.Absolute), cancellationToken).ConfigureAwait(true);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var actualContent = await result.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(true);

            Assert.Equal(expectedContent, actualContent);
        }

        [Fact]
        public async Task CanAcceptAMatchingRequestWithNonStandardDelay()
        {
            const string expectedContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse quis blandit lectus, vel facilisis odio. Morbi gravida non elit ac dignissim. Nullam at massa metus. Aenean euismod ex vitae suscipit cursus. Suspendisse vitae efficitur risus. Ut non leo nulla. Phasellus odio velit, molestie non congue nec, ornare at arcu. Fusce in interdum lectus. Pellentesque pulvinar nunc sagittis nisl porttitor lacinia. Cras quam libero, consectetur sit amet volutpat sed, gravida at turpis. Vivamus at dapibus nisi, non sollicitudin risus.";

            var cancellationToken = _testContextAccessor.Current.CancellationToken;

            var builder = new MockedHttpClientBuilder();

            using var content = new StringContent(expectedContent);

            builder
                .Configure(x => x.DefaultDelay = TimeSpan.FromMilliseconds(250))
                .Expect(HttpMethodOperation.Get, new Uri(TestUri1, UriKind.Absolute))
                .Reply(content, HttpStatusCode.OK);

            using var client = builder.Build(_loggerFactory.CreateLogger<HttpClientBasicTests>(), _garbage);

            var result = await client.GetAsync(new Uri(TestUri1, UriKind.Absolute), cancellationToken).ConfigureAwait(true);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var actualContent = await result.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(true);

            Assert.Equal(expectedContent, actualContent);
        }

        [Fact]
        public void CanCreateAClient()
        {
            var builder = new MockedHttpClientBuilder();

            using var client = builder.Build();

            Assert.NotNull(client);
        }

        [Fact]
        public async Task CanRejectAnUnexpectedRequest()
        {
            const string expectedContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse quis blandit lectus, vel facilisis odio. Morbi gravida non elit ac dignissim. Nullam at massa metus. Aenean euismod ex vitae suscipit cursus. Suspendisse vitae efficitur risus. Ut non leo nulla. Phasellus odio velit, molestie non congue nec, ornare at arcu. Fusce in interdum lectus. Pellentesque pulvinar nunc sagittis nisl porttitor lacinia. Cras quam libero, consectetur sit amet volutpat sed, gravida at turpis. Vivamus at dapibus nisi, non sollicitudin risus.";

            var cancellationToken = _testContextAccessor.Current.CancellationToken;

            var builder = new MockedHttpClientBuilder();

            using var content = new StringContent(expectedContent);

            builder
                .Expect(HttpMethodOperation.Put, new Uri(TestUri1, UriKind.Absolute))
                .Reply(content, HttpStatusCode.OK);

            using var client = builder.Build(_loggerFactory.CreateLogger<HttpClientBasicTests>(), _garbage);

            var result = await client.GetAsync(new Uri(TestUri2, UriKind.Absolute), cancellationToken).ConfigureAwait(true);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.BadGateway, result.StatusCode);
        }

        [Fact]
        public async Task CanRejectAnUnmatchingRequest()
        {
            const string expectedContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse quis blandit lectus, vel facilisis odio. Morbi gravida non elit ac dignissim. Nullam at massa metus. Aenean euismod ex vitae suscipit cursus. Suspendisse vitae efficitur risus. Ut non leo nulla. Phasellus odio velit, molestie non congue nec, ornare at arcu. Fusce in interdum lectus. Pellentesque pulvinar nunc sagittis nisl porttitor lacinia. Cras quam libero, consectetur sit amet volutpat sed, gravida at turpis. Vivamus at dapibus nisi, non sollicitudin risus.";

            var cancellationToken = _testContextAccessor.Current.CancellationToken;

            var builder = new MockedHttpClientBuilder();

            using var content = new StringContent(expectedContent);

            builder
                .Expect(HttpMethodOperation.Get, new Uri(TestUri1, UriKind.Absolute))
                .Reply(content, HttpStatusCode.OK);

            using var client = builder.Build(_loggerFactory.CreateLogger<HttpClientBasicTests>(), _garbage);

            var result = await client.GetAsync(new Uri(TestUri2, UriKind.Absolute), cancellationToken).ConfigureAwait(true);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.BadGateway, result.StatusCode);
        }

        [Fact]
        public async Task CanReplyWithAnAction()
        {
            var cancellationToken = _testContextAccessor.Current.CancellationToken;

            var builder = new MockedHttpClientBuilder();

            const string expectedContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse quis blandit lectus, vel facilisis odio. Morbi gravida non elit ac dignissim. Nullam at massa metus. Aenean euismod ex vitae suscipit cursus. Suspendisse vitae efficitur risus. Ut non leo nulla. Phasellus odio velit, molestie non congue nec, ornare at arcu. Fusce in interdum lectus. Pellentesque pulvinar nunc sagittis nisl porttitor lacinia. Cras quam libero, consectetur sit amet volutpat sed, gravida at turpis. Vivamus at dapibus nisi, non sollicitudin risus.";

            builder
                .Expect(HttpMethodOperation.Post, new Uri(TestUri1, UriKind.Absolute))
                .Reply(static(request, response) =>
                {
                    Assert.NotNull(request.Content);
                    var actual = request.Content.ReadAsStringAsync().AsSync();

                    Assert.Equal(expectedContent, actual);

                    response.StatusCode = HttpStatusCode.PreconditionFailed;
                });

            using var client = builder.Build(_loggerFactory.CreateLogger<HttpClientBasicTests>(), _garbage);

            using var content = new StringContent(expectedContent);
            var result = await client.PostAsync(new Uri(TestUri1, UriKind.Absolute), content, cancellationToken).ConfigureAwait(true);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.PreconditionFailed, result.StatusCode);
        }

        public void Dispose()
        {
            _loggerFactory.Dispose();
            _garbage.Dispose();
        }

        [Fact]
        public async Task UnconfiguredHttpClientReturnsBadGateway()
        {
            var cancellationToken = _testContextAccessor.Current.CancellationToken;

            var builder = new MockedHttpClientBuilder();

            using var client = builder.Build(_loggerFactory.CreateLogger<HttpClientBasicTests>(), _garbage);

            var result = await client.GetAsync(new Uri(TestUri1, UriKind.Absolute), cancellationToken).ConfigureAwait(true);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.BadGateway, result.StatusCode);
        }
    }
}
