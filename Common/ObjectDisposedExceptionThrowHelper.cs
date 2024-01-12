﻿// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Yextly.Common
{
    /// <summary>
    /// Provides compatibility APIs for <see cref="ObjectDisposedException"/>.
    /// </summary>
    public static class ObjectDisposedExceptionThrowHelper
    {
        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the specified <paramref name="condition"/> is <see langword="true"/>.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="instance">The object whose type's full name should be included in any resulting <see cref="ObjectDisposedException"/>.</param>
        /// <exception cref="ObjectDisposedException">The <paramref name="condition"/> is <see langword="true"/>.</exception>
        [StackTraceHidden]
        public static void ThrowIf([DoesNotReturnIf(true)] bool condition, object instance)
        {
#if !NET7_0_OR_GREATER
            if (condition)
            {
                throw new ObjectDisposedException(instance?.GetType().FullName);
            }
#else
            ObjectDisposedException.ThrowIf(condition, instance);
#endif
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the specified <paramref name="condition"/> is <see langword="true"/>.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="type">The type whose full name should be included in any resulting <see cref="ObjectDisposedException"/>.</param>
        /// <exception cref="ObjectDisposedException">The <paramref name="condition"/> is <see langword="true"/>.</exception>
        [StackTraceHidden]
        public static void ThrowIf([DoesNotReturnIf(true)] bool condition, Type type)
        {
#if !NET7_0_OR_GREATER
            if (condition)
            {
                throw new ObjectDisposedException(type?.FullName);
            }
#else
            ObjectDisposedException.ThrowIf(condition, type);
#endif
        }
    }
}
