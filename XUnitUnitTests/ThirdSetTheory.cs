// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Xunit;

namespace XUnitUnitTests
{
    public sealed class ThirdSetTheory : TheoryData<int>
    {
        public ThirdSetTheory()
        {
            for (int i = 0; i < 5; i++)
            {
                AddRow(300 + i);
            }
        }
    }
}
