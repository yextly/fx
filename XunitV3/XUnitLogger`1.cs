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
        /// <param name="diagnosticInfo">Specifies diagnostic information that must flow through the logger.</param>
        public XUnitLogger(ITestOutputHelper testOutputHelper, XUnitLoggerDiagnosticInfo diagnosticInfo) : base(testOutputHelper, new LoggerExternalScopeProvider(), typeof(T).FullName ?? "<unknown>", diagnosticInfo)
        {
        }

        /// <summary>
        /// Creates a new <see cref="XUnitLogger" /> instance with scoping support.
        /// </summary>
        /// <param name="testOutputHelper">The test output to write to.</param>
        /// <param name="scopeProvider">The scope provider to use.</param>
        /// <param name="diagnosticInfo">Specifies diagnostic information that must flow through the logger.</param>
        public XUnitLogger(ITestOutputHelper testOutputHelper, LoggerExternalScopeProvider scopeProvider, XUnitLoggerDiagnosticInfo diagnosticInfo) : base(testOutputHelper, scopeProvider, typeof(T).FullName ?? "<unknown>", diagnosticInfo)
        {
        }
    }
}
