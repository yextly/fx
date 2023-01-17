// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using Yextly.Xunit.Testing;

namespace XUnitUnitTests
{
    public sealed class Logging
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Logging(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "Not needed")]
        public void CanLog()
        {
            // This is not a test. It's just a way to se that at least it does not crash.

            using var factory = new XUnitLoggerFactory(_testOutputHelper);

            var logger = factory.CreateLogger<Logging>();

            logger.LogInformation("Test");
        }
    }
}
