// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System;
using Xunit;

namespace GlobalizationUnitTests
{
    public sealed class StringConversionTests
    {
        [Fact]
        public void CanConvertInt32ToString()
        {
            var actual = 1_000_000.ToStringInvariant();

            Assert.Equal("1000000", actual);
        }

        [Fact]
        public void CanConvertInt64ToString()
        {
            var actual = 1_000_000_000_000.ToStringInvariant();

            Assert.Equal("1000000000000", actual);
        }
    }
}
