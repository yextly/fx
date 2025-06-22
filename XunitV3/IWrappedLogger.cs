// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.Extensions.Logging;

namespace Yextly.Xunit.Testing
{
    /// <summary>
    /// Represents an instance encapsulating a <see cref="ILogger" /> instance.
    /// </summary>
    public interface IWrappedLogger
    {
        /// <summary>
        /// The encapsulated <see cref="ILogger" /> logger.
        /// </summary>
        ILogger Logger { get; }
    }
}
