// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Yextly.Xunit.Testing
{
    /// <summary>
    /// Represents a type used to configure the logging system and create instances of
    ///  <see cref="ILogger" /> from the registered Microsoft.Extensions.Logging.ILoggerProviders.
    /// </summary>
    public sealed class XUnitLoggerFactory : ILoggerFactory
    {
        private readonly ITestOutputHelper _outputHelper;

        /// <summary>
        /// Creates a <see cref="XUnitLoggerFactory"/> instance.
        /// </summary>
        /// <param name="outputHelper">The test logger output to wrap.</param>
        public XUnitLoggerFactory(ITestOutputHelper outputHelper)
        {
            ArgumentNullException.ThrowIfNull(outputHelper);
            _outputHelper = outputHelper;
        }

        /// <inheritdoc/>
        public void AddProvider(ILoggerProvider provider)
        {
            ArgumentNullException.ThrowIfNull(provider);

            // nop
        }

        /// <inheritdoc/>
        public ILogger CreateLogger(string categoryName)
        {
            return XUnitLoggingFactories.CreateLogger(_outputHelper, categoryName);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }
    }
}
