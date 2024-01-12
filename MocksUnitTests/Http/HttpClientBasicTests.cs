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
using Xunit.Abstractions;
using Yextly.Common;
using Yextly.Testing.Mocks.Http;
using Yextly.Xunit.Testing;

namespace MocksUnitTests.Http
{
    public sealed class HttpClientBasicTests : IDisposable
    {
        private readonly IDisposableProducerConsumerCollection<IDisposable> _garbage;
        private readonly XUnitLoggerFactory _loggerFactory;
        private readonly ITestOutputHelper _testOutputHelper;

        public HttpClientBasicTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _loggerFactory = new XUnitLoggerFactory(_testOutputHelper);
            _garbage = new ConcurrentBag<IDisposable>().AsDisposableCollection();
        }

        [Fact]
        public async Task CanAcceptAMatchingRequest()
        {
            const string expectedContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse quis blandit lectus, vel facilisis odio. Morbi gravida non elit ac dignissim. Nullam at massa metus. Aenean euismod ex vitae suscipit cursus. Suspendisse vitae efficitur risus. Ut non leo nulla. Phasellus odio velit, molestie non congue nec, ornare at arcu. Fusce in interdum lectus. Pellentesque pulvinar nunc sagittis nisl porttitor lacinia. Cras quam libero, consectetur sit amet volutpat sed, gravida at turpis. Vivamus at dapibus nisi, non sollicitudin risus.";

            var builder = new HttpClientMockBuilder();

            using var content = new StringContent(expectedContent);

            builder
                .Expect(HttpMethodOperation.Get, new Uri("https://www.website1.blackhole/test.php?q=123", UriKind.Absolute))
                .Reply(content, HttpStatusCode.OK);

            using var client = builder.Build(_loggerFactory.CreateLogger<HttpClientBasicTests>(), _garbage);

            var result = await client.GetAsync(new Uri("https://www.website1.blackhole/test.php?q=123", UriKind.Absolute)).ConfigureAwait(true);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var actualContent = await result.Content.ReadAsStringAsync().ConfigureAwait(true);

            Assert.Equal(expectedContent, actualContent);
        }

        [Fact]
        public void CanCreateAClient()
        {
            var builder = new HttpClientMockBuilder();

            using var client = builder.Build();

            Assert.NotNull(client);
        }

        [Fact]
        public async Task CanRejectAnUnexpectedRequest()
        {
            const string expectedContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse quis blandit lectus, vel facilisis odio. Morbi gravida non elit ac dignissim. Nullam at massa metus. Aenean euismod ex vitae suscipit cursus. Suspendisse vitae efficitur risus. Ut non leo nulla. Phasellus odio velit, molestie non congue nec, ornare at arcu. Fusce in interdum lectus. Pellentesque pulvinar nunc sagittis nisl porttitor lacinia. Cras quam libero, consectetur sit amet volutpat sed, gravida at turpis. Vivamus at dapibus nisi, non sollicitudin risus.";

            var builder = new HttpClientMockBuilder();

            using var content = new StringContent(expectedContent);

            builder
                .Expect(HttpMethodOperation.Put, new Uri("https://www.website1.blackhole/test.php?q=123", UriKind.Absolute))
                .Reply(content, HttpStatusCode.OK);

            using var client = builder.Build(_loggerFactory.CreateLogger<HttpClientBasicTests>(), _garbage);

            var result = await client.GetAsync(new Uri("https://www.website1.blackhole/test.php?q=999", UriKind.Absolute)).ConfigureAwait(true);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.BadGateway, result.StatusCode);
        }

        [Fact]
        public async Task CanRejectAnUnmatchingRequest()
        {
            const string expectedContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse quis blandit lectus, vel facilisis odio. Morbi gravida non elit ac dignissim. Nullam at massa metus. Aenean euismod ex vitae suscipit cursus. Suspendisse vitae efficitur risus. Ut non leo nulla. Phasellus odio velit, molestie non congue nec, ornare at arcu. Fusce in interdum lectus. Pellentesque pulvinar nunc sagittis nisl porttitor lacinia. Cras quam libero, consectetur sit amet volutpat sed, gravida at turpis. Vivamus at dapibus nisi, non sollicitudin risus.";

            var builder = new HttpClientMockBuilder();

            using var content = new StringContent(expectedContent);

            builder
                .Expect(HttpMethodOperation.Get, new Uri("https://www.website1.blackhole/test.php?q=123", UriKind.Absolute))
                .Reply(content, HttpStatusCode.OK);

            using var client = builder.Build(_loggerFactory.CreateLogger<HttpClientBasicTests>(), _garbage);

            var result = await client.GetAsync(new Uri("https://www.website1.blackhole/test.php?q=999", UriKind.Absolute)).ConfigureAwait(true);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.BadGateway, result.StatusCode);
        }

        public void Dispose()
        {
            _loggerFactory.Dispose();
            _garbage.Dispose();
        }

        [Fact]
        public async Task UnconfiguredHttpClientResturnsBadGateway()
        {
            var builder = new HttpClientMockBuilder();

            using var client = builder.Build(_loggerFactory.CreateLogger<HttpClientBasicTests>(), _garbage);

            var result = await client.GetAsync(new Uri("https://www.website1.blackhole/test.php?q=123", UriKind.Absolute)).ConfigureAwait(true);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.BadGateway, result.StatusCode);
        }
    }
}
