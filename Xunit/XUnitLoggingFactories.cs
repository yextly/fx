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
    /// Contains factory methods for creating <see cref="ILogger{TCategoryName}"/> from <see cref="ITestOutputHelper"/>.
    /// </summary>
    public static class XUnitLoggingFactories
    {
        /// <summary>
        /// Creates a logger with category.
        /// </summary>
        /// <param name="testOutputHelper">The XUnit test output helper.</param>
        /// <param name="categoryName">The name of the category.</param>
        /// <returns></returns>
        public static ILogger CreateLogger(ITestOutputHelper testOutputHelper, string categoryName) => new XUnitLogger(testOutputHelper, new LoggerExternalScopeProvider(), categoryName);

        /// <summary>
        /// Creates a logger without category.
        /// </summary>
        /// <param name="testOutputHelper">The XUnit test output helper.</param>
        /// <returns></returns>
        public static ILogger CreateLogger(ITestOutputHelper testOutputHelper) => new XUnitLogger(testOutputHelper, new LoggerExternalScopeProvider(), string.Empty);

        /// <summary>
        /// Creates a logger with category.
        /// </summary>
        /// <param name="testOutputHelper">The XUnit test output helper.</param>
        /// <typeparam name="T">The category name.</typeparam>
        /// <returns></returns>
        public static ILogger<T> CreateLogger<T>(ITestOutputHelper testOutputHelper) => new XUnitLogger<T>(testOutputHelper, new LoggerExternalScopeProvider());

        /// <summary>
        /// Tries to create a <see cref="ILogger{TCategoryName}"/> from the provided <paramref name="testOutputHelper"/>.
        /// When not possible, a new logger is created.
        /// </summary>
        /// <param name="testOutputHelper">The Xunit test output helper or a wrapped <see cref="IWrappedLogger"/> instance.</param>
        /// <returns></returns>
        public static ILogger CreateOrUnwrap(ITestOutputHelper testOutputHelper)
        {
            if (testOutputHelper is IWrappedLogger w)
                return w.Logger;
            else
                return CreateLogger(testOutputHelper);
        }

        /// <summary>
        /// Tries to create a <see cref="ILogger{TCategoryName}"/> from the provided <paramref name="testOutputHelper"/>.
        /// When not possible, a new logger is created.
        /// </summary>
        /// <typeparam name="T">The category name.</typeparam>
        /// <param name="testOutputHelper">The Xunit test output helper or a wrapped <see cref="IWrappedLogger"/> instance.</param>
        /// <returns></returns>
        public static ILogger<T> CreateOrUnwrap<T>(ITestOutputHelper testOutputHelper)
        {
            ArgumentNullException.ThrowIfNull(testOutputHelper);

            if (testOutputHelper is IWrappedLogger w && w.Logger is ILogger<T> ret)
                return ret;
            else
                return CreateLogger<T>(testOutputHelper);
        }
    }
}
