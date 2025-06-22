// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System;
using System.Runtime.CompilerServices;

namespace CommonUnitTests
{
    internal static class CompatibilityShims
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IAsyncDisposable AsAsyncDisposable<T>(this T instance, out T outInstance) where T : notnull, IAsyncDisposable
        {
            // Do not validate instance here, otherwise the jitter will not inline the site.

            outInstance = instance;

            return instance;
        }
    }
}
