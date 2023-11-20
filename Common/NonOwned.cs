// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Common
{
    /// <summary>
    /// Represents an object encapsulating a disposable object which must not be disposed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// Constructs a new <see cref="NonOwned{T}"/>
    /// </remarks>
    /// <param name="value">The value to be wrapped.</param>
    public sealed class NonOwned<T>(T value) : IDisposable where T : IDisposable
    {
        /// <summary>
        /// Returns a reference to the wrapped object.
        /// </summary>
        /// <remarks>Since the returned instance is not owned, you must not dispose it.</remarks>
        public T Value { get; } = value;

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not dispose the inner instance since it is not owned
        }
    }
}
