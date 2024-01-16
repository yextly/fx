// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace CommonUnitTests
{
    internal sealed class ObjectStealer<T>(T value)
    {
        public T Value { get; } = value;
    }
}
