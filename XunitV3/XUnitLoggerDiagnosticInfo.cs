// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Diagnostics;

namespace Yextly.Xunit.Testing
{
    /// <summary>
    /// Contains diagnostics information for the XUnit logger.
    /// </summary>
    public sealed record XUnitLoggerDiagnosticInfo
    {
        /// <summary>
        /// Contains the call stack trace when the logger/logger factory was created.
        /// </summary>
        public required StackTrace CreationStackTrace { get; init; }
    }
}
