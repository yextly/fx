// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Xunit;

namespace XUnitV3UnitTests
{
    public sealed class ThirdSetTheory : TheoryData<int>
    {
        public ThirdSetTheory()
        {
            for (int i = 0; i < 5; i++)
            {
                Add(300 + i);
            }
        }
    }
}
