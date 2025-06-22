// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Xunit;

namespace XUnitV3UnitTests
{
    public sealed class FirstSetTheory : TheoryData<int>
    {
        public FirstSetTheory()
        {
            for (int i = 0; i < 13; i++)
            {
                Add(100 + i);
            }
        }
    }
}
