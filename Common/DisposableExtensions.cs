// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Yextly.Common;

namespace System
{
    /// <summary>
    /// Contains extensions for <see cref="IDisposable"/>.
    /// </summary>
    public static class DisposableExtensions
    {
        /// <summary>
        /// Creates a wrapper against the porvided instance so that it will not be diposed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The instance to wrap.</param>
        /// <returns></returns>
        public static NonOwned<T> AsNonOwned<T>(this T value) where T : IDisposable
        {
            ArgumentNullException.ThrowIfNull(value);

            return new NonOwned<T>(value);
        }
    }
}
