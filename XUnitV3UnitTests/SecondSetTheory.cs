// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Xunit;

namespace XUnitV3UnitTests
{
    public sealed class SecondSetTheory : TheoryData<int>
    {
        public SecondSetTheory()
        {
            for (int i = 0; i < 7; i++)
            {
                Add(200 + i);
            }
        }
    }
}
