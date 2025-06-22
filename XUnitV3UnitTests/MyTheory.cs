// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Yextly.Xunit;

namespace XUnitV3UnitTests
{
    public sealed class MyTheory : CombinedTheoryData
    {
        public MyTheory() : base(typeof(FirstSetTheory), typeof(SecondSetTheory), typeof(ThirdSetTheory))
        {
        }
    }
}
