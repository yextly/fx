// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace ScriptingServicesTests
{
    internal sealed class Context2 : IContext2
    {
        public int Value { get; private set; }

        public void SideEffect1(int value)
        {
            Value += 1000 * value;
        }

        public void SideEffect2(int value)
        {
            Value += 10000 * value;
        }
    }
}
