// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Runtime.CompilerServices;
using Yextly.Common;

namespace System
{
    /// <summary>
    /// Contains extensions for <see cref="IDisposable"/>.
    /// </summary>
    public static class DisposableExtensions
    {
        ///// <summary>
        ///// Implements the trick described in https://github.com/dotnet/csharplang/discussions/2661 to limit the scope of an async disposable variable.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="instance">The instance that must be disposed when finished.</param>
        ///// <param name="outInstance">The same instance as <paramref name="instance"/>.</param>
        ///// <returns>Returns <paramref name="instance"/>.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static IAsyncDisposable AsAsyncDisposable<T>(this T instance, out T outInstance) where T : notnull, IAsyncDisposable
        //{
        //    // do not validate instance here, otherwise the jitter will not inline the site.

        //    outInstance = instance;

        //    return instance;
        //}

        /// <summary>
        /// Creates a wrapper against the provided instance so that it will not be disposed.
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
