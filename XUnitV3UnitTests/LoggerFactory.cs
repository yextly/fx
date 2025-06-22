// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Yextly.Xunit.Testing;

namespace XUnitV3UnitTests
{
    public sealed partial class LoggerFactory
    {
        private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(3);

        private readonly ITestOutputHelper _testOutputHelper;

        public LoggerFactory(ITestOutputHelper testOutputHelper)
        {
            ArgumentNullException.ThrowIfNull(testOutputHelper);

            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CanCreateTheProvider()
        {
            using var factory = new XUnitLoggerFactory(_testOutputHelper);

            var value = factory.CreateLogger<LoggerFactory>();
            Assert.NotNull(value);
        }

        [Fact]
        public async Task CanLogExceptions()
        {
            var assertHelper = new AssertTestOutputHelper();
            var router = new DualTestOutputHelper(_testOutputHelper, assertHelper);
            using var factory = new XUnitLoggerFactory(router);

            var logger = factory.CreateLogger<LoggerFactory>();

            var actuator = new Actuator(logger);
            try
            {
                throw new InvalidOperationException("OP1");
            }
            catch (InvalidOperationException ex)
            {
                actuator.LogWarning1("DEF", ex);
            }

            using var source = new CancellationTokenSource(_timeout);

            const string expected = @"(?s)^warn \[XUnitV3UnitTests\.LoggerFactory, 2, LogWarning1\] M2#DEF#[\n|\r\n|\n\r]System\.InvalidOperationException\: OP1[\n|\r\n|\n\r]\s+at XUnitV3UnitTests\.LoggerFactory\.CanLogExceptions\(\) in (.*)$";

            var actual = await assertHelper.GetValueAsync(source.Token).ConfigureAwait(true);

            Assert.Matches(expected, actual);
        }

        [Fact]
        public async Task CanLogInformation()
        {
            var assertHelper = new AssertTestOutputHelper();
            var router = new DualTestOutputHelper(_testOutputHelper, assertHelper);
            using var factory = new XUnitLoggerFactory(router);

            var logger = factory.CreateLogger<LoggerFactory>();

            var actuator = new Actuator(logger);

            actuator.LogInfo1("ABC");

            using var source = new CancellationTokenSource(_timeout);

            const string expected = "info [XUnitV3UnitTests.LoggerFactory, 1, LogInfo1] M1#ABC#";

            var actual = await assertHelper.GetValueAsync(source.Token).ConfigureAwait(true);

            Assert.Equal(expected, actual);
        }

        private sealed partial class Actuator
        {
            private readonly ILogger _logger;

            public Actuator(ILogger logger)
            {
                ArgumentNullException.ThrowIfNull(logger);

                _logger = logger;
            }

            [LoggerMessage(Message = "M1#{Message}#", Level = LogLevel.Information, EventId = 1)]
            public partial void LogInfo1(string message);

            [LoggerMessage(Message = "M2#{Message}#", Level = LogLevel.Warning, EventId = 2)]
            public partial void LogWarning1(string message, Exception exception);
        }
    }
}
