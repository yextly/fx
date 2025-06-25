// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Diagnostics;

namespace Yextly.Xunit.Testing
{
    /// <summary>
    /// Contains factory methods for creating <see cref="XUnitLoggerDiagnosticInfo" />.
    /// </summary>
    public static class LoggerDiagnostics
    {
        /// <summary>
        /// Creates and initializes a new <see cref="XUnitLoggerDiagnosticInfo" /> instance.
        /// </summary>
        public static XUnitLoggerDiagnosticInfo CreateInitialDiagnosticInfo()
        {
            var stack = new StackTrace(1, false);

            return new XUnitLoggerDiagnosticInfo
            {
                CreationStackTrace = stack,
            };
        }

        /// <summary>
        /// Produces a string representation of the provided <see cref="XUnitLoggerDiagnosticInfo" />.
        /// </summary>
        /// <param name="diagnosticInfo">The value whose string representation needs to be computed.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "By design")]
        public static string GetDiagnosticInformation(XUnitLoggerDiagnosticInfo diagnosticInfo)
        {
            if (diagnosticInfo == null)
            {
                return "<no information available>";
            }

            try
            {
                var stack = diagnosticInfo.CreationStackTrace;
                if (stack == null || stack.FrameCount == 0)
                {
                    return "Created: <no stack trace available>";
                }

                var value = stack.ToString();
                return "Created: " + value;
            }
            catch
            {
                // We saw this in certain convoluted scenarios. Therefore, we don't crash the testing engine.
                return "<error while computing diagnostic information>";
            }
        }
    }
}
