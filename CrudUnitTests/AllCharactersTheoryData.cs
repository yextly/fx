// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Linq;
using Xunit;

namespace CrudUnitTests
{
    public sealed class AllCharactersTheoryData : TheoryData<int>
    {
        public AllCharactersTheoryData()
        {
            foreach (var item in Enumerable.Range(1, 905))
            {
                Add(item);
            }
        }
    }
}
