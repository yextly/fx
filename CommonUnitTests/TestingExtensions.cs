// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace CommonUnitTests
{
    internal static class TestingExtensions
    {
        public static ObjectStealer<T> Steal<T>(this T instance)
        {
            return new ObjectStealer<T>(instance);
        }
    }
}
