// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;
using Xunit.Abstractions;

namespace Yextly.Xunit.Testing
{
    /// <summary>
    /// Represents a logger which writes to the provided <see cref="ITestOutputHelper"/>.
    /// </summary>
    public class XUnitLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly LoggerExternalScopeProvider _scopeProvider;
        private readonly ITestOutputHelper _testOutputHelper;

        /// <summary>
        /// Creates a new <see cref="XUnitLogger"/> instance.
        /// </summary>
        /// <param name="testOutputHelper">The test output to write to.</param>
        /// <param name="scopeProvider">The scope provider to integrate the logs with.</param>
        /// <param name="categoryName">Specifies the category of this logger.</param>
        public XUnitLogger(ITestOutputHelper testOutputHelper, LoggerExternalScopeProvider scopeProvider, string categoryName)
        {
            ArgumentNullException.ThrowIfNull(testOutputHelper);
            ArgumentNullException.ThrowIfNull(scopeProvider);
            ArgumentNullException.ThrowIfNull(categoryName);

            _testOutputHelper = testOutputHelper;
            _scopeProvider = scopeProvider;
            _categoryName = categoryName;
        }

        /// <inheritdoc/>
        public IDisposable BeginScope<TState>(TState state) => _scopeProvider.Push(state);

        /// <inheritdoc/>
        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        /// <inheritdoc/>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (formatter is null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var sb = new StringBuilder();
            sb.Append(GetLogLevelString(logLevel))
              .Append(" [").Append(_categoryName).Append("] ")
              .Append(formatter(state, exception));

            if (exception != null)
            {
                sb.Append('\n').Append(exception);
            }

            // Append scopes
            _scopeProvider.ForEachScope((scope, state) =>
            {
                state.Append("\n => ");
                state.Append(scope);
            }, sb);

            _testOutputHelper.WriteLine(sb.ToString());
            Debug.WriteLine(sb.ToString());
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => "trce",
                LogLevel.Debug => "dbug",
                LogLevel.Information => "info",
                LogLevel.Warning => "warn",
                LogLevel.Error => "fail",
                LogLevel.Critical => "crit",
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
            };
        }
    }
}
