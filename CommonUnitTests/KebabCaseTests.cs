// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System;
using Xunit;

namespace CommonUnitTests
{
    public sealed class KebabCaseTests
    {
        [ClassData(typeof(KebabTheoryData))]
        [Theory]
        public void CanGenerateKebabCase(string? expected, string? input)
        {
            var actual = input.ToKebab();

            Assert.Equal(expected, actual);
        }
    }
}
