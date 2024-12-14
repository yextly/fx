﻿// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Yextly.Common;
using Yextly.Tasks;
using Yextly.Testing.Mocks.Http;
using Yextly.Xunit.Testing;

namespace MocksUnitTests.Http
{
    public sealed class HttpClientFactoryBasicTests : IDisposable
    {
        private readonly IDisposableProducerConsumerCollection<IDisposable> _garbage;
        private readonly XUnitLoggerFactory _loggerFactory;
        private readonly ITestOutputHelper _testOutputHelper;

        public HttpClientFactoryBasicTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _loggerFactory = new XUnitLoggerFactory(_testOutputHelper);
            _garbage = new ConcurrentBag<IDisposable>().AsDisposableCollection();
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "Not necessary")]
        public async Task CanAcceptAMatchingRequest()
        {
            const string expectedContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse quis blandit lectus, vel facilisis odio. Morbi gravida non elit ac dignissim. Nullam at massa metus. Aenean euismod ex vitae suscipit cursus. Suspendisse vitae efficitur risus. Ut non leo nulla. Phasellus odio velit, molestie non congue nec, ornare at arcu. Fusce in interdum lectus. Pellentesque pulvinar nunc sagittis nisl porttitor lacinia. Cras quam libero, consectetur sit amet volutpat sed, gravida at turpis. Vivamus at dapibus nisi, non sollicitudin risus.";

            using var content = new StringContent(expectedContent);

            var collection = new ServiceCollection();

            var timeout = TimeSpan.FromSeconds(30);

            collection.AddMockedHttpClient("Client1")
                .Expect(HttpMethodOperation.Get, new Uri("https://www.website1.blackhole/test.php?q=123", UriKind.Absolute))
                .Reply(content, HttpStatusCode.OK);

            collection.AddHttpClient("Client1")
                .AddPolicyHandler((serviceProvider, _) =>
                    HttpPolicyExtensions.HandleTransientHttpError()
                        .WaitAndRetryAsync(GetIntervals(), onRetry: (outcome, sleepDuration, attemptNumber, _) =>
                        {
                            var logger = serviceProvider.GetRequiredService<ILogger<HttpClient>>();

                            var message = outcome?.Exception?.Message ?? "No message available";
                            var result = outcome?.Result?.StatusCode;
                            var resultMessage = result == null ? "Not available" : Enum.GetName<HttpStatusCode>((HttpStatusCode)result) ?? "Unknown result";

                            logger.LogWarning(outcome?.Exception, "HTTP transient error {Message}, result {Result}, retrying in {Interval}. This is attempt {Number}.", message, resultMessage, sleepDuration, attemptNumber);
                        }))
                .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(timeout));

            using var provider = collection.BuildServiceProvider();

            using var scope = provider.CreateScope();
            var services = scope.ServiceProvider;

            var factory = services.GetRequiredService<IHttpClientFactory>();
            using var client = factory.CreateClient("Client1");

            var result = await client.GetAsync(new Uri("https://www.website1.blackhole/test.php?q=123", UriKind.Absolute)).ConfigureAwait(true);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var actualContent = await result.Content.ReadAsStringAsync().ConfigureAwait(true);

            Assert.Equal(expectedContent, actualContent);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "Not necessary")]
        public async Task CanAcceptAMatchingRequestFromDefaultClient()
        {
            const string expectedContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse quis blandit lectus, vel facilisis odio. Morbi gravida non elit ac dignissim. Nullam at massa metus. Aenean euismod ex vitae suscipit cursus. Suspendisse vitae efficitur risus. Ut non leo nulla. Phasellus odio velit, molestie non congue nec, ornare at arcu. Fusce in interdum lectus. Pellentesque pulvinar nunc sagittis nisl porttitor lacinia. Cras quam libero, consectetur sit amet volutpat sed, gravida at turpis. Vivamus at dapibus nisi, non sollicitudin risus.";

            using var content = new StringContent(expectedContent);

            var collection = new ServiceCollection();

            var timeout = TimeSpan.FromSeconds(30);

            collection.AddMockedHttpClient()
                .Expect(HttpMethodOperation.Get, new Uri("https://www.website1.blackhole/test.php?q=123", UriKind.Absolute))
                .Reply(content, HttpStatusCode.OK);

            collection.AddHttpClient<HttpClient>()
                .AddPolicyHandler((serviceProvider, _) =>
                    HttpPolicyExtensions.HandleTransientHttpError()
                        .WaitAndRetryAsync(GetIntervals(), onRetry: (outcome, sleepDuration, attemptNumber, _) =>
                        {
                            var logger = serviceProvider.GetRequiredService<ILogger<HttpClient>>();

                            var message = outcome?.Exception?.Message ?? "No message available";
                            var result = outcome?.Result?.StatusCode;
                            var resultMessage = result == null ? "Not available" : Enum.GetName<HttpStatusCode>((HttpStatusCode)result) ?? "Unknown result";

                            logger.LogWarning(outcome?.Exception, "HTTP transient error {Message}, result {Result}, retrying in {Interval}. This is attempt {Number}.", message, resultMessage, sleepDuration, attemptNumber);
                        }))
                .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(timeout));

            using var provider = collection.BuildServiceProvider();

            using var scope = provider.CreateScope();
            var services = scope.ServiceProvider;

            var factory = services.GetRequiredService<IHttpClientFactory>();
            using var client = factory.CreateClient("Client1");

            var result = await client.GetAsync(new Uri("https://www.website1.blackhole/test.php?q=123", UriKind.Absolute)).ConfigureAwait(true);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var actualContent = await result.Content.ReadAsStringAsync().ConfigureAwait(true);

            Assert.Equal(expectedContent, actualContent);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "Not necessary")]
        public async Task CanRejectUnknownDefaultClient()
        {
            const string expectedContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse quis blandit lectus, vel facilisis odio. Morbi gravida non elit ac dignissim. Nullam at massa metus. Aenean euismod ex vitae suscipit cursus. Suspendisse vitae efficitur risus. Ut non leo nulla. Phasellus odio velit, molestie non congue nec, ornare at arcu. Fusce in interdum lectus. Pellentesque pulvinar nunc sagittis nisl porttitor lacinia. Cras quam libero, consectetur sit amet volutpat sed, gravida at turpis. Vivamus at dapibus nisi, non sollicitudin risus.";

            using var content = new StringContent(expectedContent);

            var collection = new ServiceCollection();

            var timeout = TimeSpan.FromSeconds(30);

            collection.AddMockedHttpClient("Client1")
                .Expect(HttpMethodOperation.Get, new Uri("https://www.website1.blackhole/test.php?q=123", UriKind.Absolute))
                .Reply(content, HttpStatusCode.OK);

            collection.AddHttpClient("Client1")
                .AddPolicyHandler((serviceProvider, _) =>
                    HttpPolicyExtensions.HandleTransientHttpError()
                        .WaitAndRetryAsync(GetIntervals(), onRetry: (outcome, sleepDuration, attemptNumber, _) =>
                        {
                            var logger = serviceProvider.GetRequiredService<ILogger<HttpClient>>();

                            var message = outcome?.Exception?.Message ?? "No message available";
                            var result = outcome?.Result?.StatusCode;
                            var resultMessage = result == null ? "Not available" : Enum.GetName<HttpStatusCode>((HttpStatusCode)result) ?? "Unknown result";

                            logger.LogWarning(outcome?.Exception, "HTTP transient error {Message}, result {Result}, retrying in {Interval}. This is attempt {Number}.", message, resultMessage, sleepDuration, attemptNumber);
                        }))
                .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(timeout));

            using var provider = collection.BuildServiceProvider();

            using var scope = provider.CreateScope();
            var services = scope.ServiceProvider;

            var factory = services.GetRequiredService<IHttpClientFactory>();
            using var client = factory.CreateClient();

            var result = await client.GetAsync(new Uri("https://www.website1.blackhole/test.php?q=123", UriKind.Absolute)).ConfigureAwait(true);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.BadGateway, result.StatusCode);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "Not necessary")]
        public async Task CanRejectUnknownNamedClients()
        {
            const string expectedContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse quis blandit lectus, vel facilisis odio. Morbi gravida non elit ac dignissim. Nullam at massa metus. Aenean euismod ex vitae suscipit cursus. Suspendisse vitae efficitur risus. Ut non leo nulla. Phasellus odio velit, molestie non congue nec, ornare at arcu. Fusce in interdum lectus. Pellentesque pulvinar nunc sagittis nisl porttitor lacinia. Cras quam libero, consectetur sit amet volutpat sed, gravida at turpis. Vivamus at dapibus nisi, non sollicitudin risus.";

            using var content = new StringContent(expectedContent);

            var collection = new ServiceCollection();

            var timeout = TimeSpan.FromSeconds(30);

            collection.AddMockedHttpClient("Client1")
                .Expect(HttpMethodOperation.Get, new Uri("https://www.website1.blackhole/test.php?q=123", UriKind.Absolute))
                .Reply(content, HttpStatusCode.OK);

            collection.AddHttpClient("Client1")
                .AddPolicyHandler((serviceProvider, _) =>
                    HttpPolicyExtensions.HandleTransientHttpError()
                        .WaitAndRetryAsync(GetIntervals(), onRetry: (outcome, sleepDuration, attemptNumber, _) =>
                        {
                            var logger = serviceProvider.GetRequiredService<ILogger<HttpClient>>();

                            var message = outcome?.Exception?.Message ?? "No message available";
                            var result = outcome?.Result?.StatusCode;
                            var resultMessage = result == null ? "Not available" : Enum.GetName<HttpStatusCode>((HttpStatusCode)result) ?? "Unknown result";

                            logger.LogWarning(outcome?.Exception, "HTTP transient error {Message}, result {Result}, retrying in {Interval}. This is attempt {Number}.", message, resultMessage, sleepDuration, attemptNumber);
                        }))
                .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(timeout));

            using var provider = collection.BuildServiceProvider();

            using var scope = provider.CreateScope();
            var services = scope.ServiceProvider;

            var factory = services.GetRequiredService<IHttpClientFactory>();
            using var client = factory.CreateClient("Client2");

            var result = await client.GetAsync(new Uri("https://www.website1.blackhole/test.php?q=123", UriKind.Absolute)).ConfigureAwait(true);

            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.BadGateway, result.StatusCode);
        }

        public void Dispose()
        {
            _loggerFactory.Dispose();
            _garbage.Dispose();
        }

        private static IEnumerable<TimeSpan> GetIntervals()
        {
            return Enumerable.Repeat(TimeSpan.FromMilliseconds(100), 1000);
        }
    }
}
