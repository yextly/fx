// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Xunit;

namespace XUnitUnitTests
{
    public sealed class FirstSetTheory : TheoryData<int>
    {
        public FirstSetTheory()
        {
            for (int i = 0; i < 13; i++)
            {
                AddRow(100 + i);
            }
        }
    }
}
