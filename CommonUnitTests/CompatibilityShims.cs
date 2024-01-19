// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Runtime.CompilerServices;
using System;

namespace CommonUnitTests
{
    internal static class CompatibilityShims
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IAsyncDisposable AsAsyncDisposable<T>(this T instance, out T outInstance) where T : notnull, IAsyncDisposable
        {
            // do not validate instance here, otherwise the jitter will not inline the site.

            outInstance = instance;

            return instance;
        }
    }
}
