// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Yextly.Xunit;

namespace XUnitUnitTests
{
    public sealed class MyTheory : CombinedTheoryData
    {
        public MyTheory() : base(typeof(FirstSetTheory), typeof(SecondSetTheory), typeof(ThirdSetTheory))
        {
        }
    }
}
