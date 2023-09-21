// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.Logging;
using System;

namespace CrudUnitTests
{
    internal sealed class ProxyUtilities
    {
        public static ILogger<Data1Controller> CreateControllerLogger()
        {
            return new ControllerLogger();
        }

        private sealed class ControllerLogger : ILogger<Data1Controller>, IDisposable
        {
            public IDisposable BeginScope<TState>(TState state)
            {
                return this;
            }

            public void Dispose()
            {
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return false;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
            }
        }
    }
}
