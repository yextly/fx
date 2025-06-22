// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.Logging;
using Xunit;

namespace Yextly.Xunit.Testing
{
    /// <inheritdoc />
    public sealed class XUnitLogger<T> : XUnitLogger, ILogger<T>
    {
        /// <summary>
        /// Creates a new <see cref="XUnitLogger" /> instance with default values.
        /// </summary>
        /// <param name="testOutputHelper">The test output to write to.</param>
        public XUnitLogger(ITestOutputHelper testOutputHelper) : base(testOutputHelper, new LoggerExternalScopeProvider(), typeof(T).FullName ?? "<unknown>")
        {
        }

        /// <summary>
        /// Creates a new <see cref="XUnitLogger" /> instance with scoping support.
        /// </summary>
        /// <param name="testOutputHelper">The test output to write to.</param>
        /// <param name="scopeProvider">The scope provider to use.</param>
        public XUnitLogger(ITestOutputHelper testOutputHelper, LoggerExternalScopeProvider scopeProvider) : base(testOutputHelper, scopeProvider, typeof(T).FullName ?? "<unknown>")
        {
        }
    }
}
