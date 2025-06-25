// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace Yextly.Xunit.Testing
{
    /// <summary>
    /// Represents a logger which writes to the provided <see cref="ITestOutputHelper" />.
    /// </summary>
    public class XUnitLogger : ILogger
    {
        private static readonly Lazy<ObjectPool<StringBuilder>> _pool = new(InitializePool, LazyThreadSafetyMode.PublicationOnly);
        private readonly string _categoryName;
        private readonly XUnitLoggerDiagnosticInfo _diagnosticInfo;
        private readonly LoggerExternalScopeProvider _scopeProvider;
        private readonly ITestOutputHelper _testOutputHelper;

        /// <summary>
        /// Creates a new <see cref="XUnitLogger" /> instance.
        /// </summary>
        /// <param name="testOutputHelper">The test output to write to.</param>
        /// <param name="scopeProvider">The scope provider to integrate the logs with.</param>
        /// <param name="categoryName">Specifies the category of this logger.</param>
        /// <param name="diagnosticInfo">Specifies diagnostic information that must flow through the logger.</param>
        public XUnitLogger(ITestOutputHelper testOutputHelper, LoggerExternalScopeProvider scopeProvider, string categoryName, XUnitLoggerDiagnosticInfo diagnosticInfo)
        {
            ArgumentNullException.ThrowIfNull(testOutputHelper);
            ArgumentNullException.ThrowIfNull(scopeProvider);
            ArgumentNullException.ThrowIfNull(categoryName);
            ArgumentNullException.ThrowIfNull(diagnosticInfo);

            _testOutputHelper = testOutputHelper;
            _scopeProvider = scopeProvider;
            _categoryName = categoryName;
            _diagnosticInfo = diagnosticInfo;
        }

        /// <inheritdoc />
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return _scopeProvider.Push(state);
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            ArgumentNullException.ThrowIfNull(formatter);

            var pool = _pool.Value;

            var sb = pool.Get();
            try
            {
                sb.Append(GetLogLevelString(logLevel));
                sb.Append(" [");
                sb.Append(_categoryName);

                if (eventId.Id != 0)
                {
                    sb.Append(", ");
                    sb.Append(eventId.Id);
                    sb.Append(", ");
                    sb.Append(eventId.Name);
                }

                sb.Append("] ");
                sb.Append(formatter(state, exception));

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
            catch (Exception ex)
            {
                var builder = new StringBuilder();
                builder.AppendLine("Failed to dispatch a log to the inner log provider.");
                builder.AppendLine("Follows the logger diagnostic inforation:");
                builder.AppendLine(LoggerDiagnostics.GetDiagnosticInformation(_diagnosticInfo));

                throw new InvalidOperationException(builder.ToString(), ex);
            }
            finally
            {
                pool.Return(sb);
            }
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

        private static ObjectPool<StringBuilder> InitializePool()
        {
            var provider = new DefaultObjectPoolProvider();
            var policy = new StringBuilderPooledObjectPolicy();
            return provider.Create(policy);
        }
    }
}
