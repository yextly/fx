// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Xunit;

namespace XUnitUnitTests
{
    public sealed class SecondSetTheory : TheoryData<int>
    {
        public SecondSetTheory()
        {
            for (int i = 0; i < 7; i++)
            {
                AddRow(200 + i);
            }
        }
    }
}
