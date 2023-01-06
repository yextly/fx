// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Reflection;
using Xunit;

namespace ReflectionUnitTests
{
    public sealed class AssemblyTests
    {
        [Fact]
        public void CanExtractTargetFramework()
        {
            var targetFramework = typeof(AssemblyTests).Assembly.GetTargetFramework();

            Assert.NotNull(targetFramework);
        }
    }
}
