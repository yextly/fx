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
    public sealed class NonOwned<T> : IDisposable where T : IDisposable
    {
        /// <summary>
        /// Constructs a new <see cref="NonOwned{T}"/>
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        public NonOwned(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Returns a reference to the wrapped object.
        /// </summary>
        /// <remarks>Since the returned instance is not owned, you must not dispose it.</remarks>
        public T Value { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not dispose the inner instance since it is not owned
        }
    }
}
