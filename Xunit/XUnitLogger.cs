// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
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
        private static readonly Lazy<ObjectPool<StringBuilder>> _pool = new(InitializePool, LazyThreadSafetyMode.PublicationOnly);

        private static ObjectPool<StringBuilder> InitializePool()
        {
            var provider = new DefaultObjectPoolProvider();
            var policy = new StringBuilderPooledObjectPolicy();
            return provider.Create(policy);
        }

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

#if NET6_0

        /// <inheritdoc/>
        public IDisposable BeginScope<TState>(TState state) => _scopeProvider.Push(state);

#elif NET7_0_OR_GREATER
        /// <inheritdoc/>
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => _scopeProvider.Push(state);
#endif

        /// <inheritdoc/>
        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        /// <inheritdoc/>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            ArgumentNullException.ThrowIfNull(formatter);

            var pool = _pool.Value;

            var sb = pool.Get();
            try
            {
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
    }
}
