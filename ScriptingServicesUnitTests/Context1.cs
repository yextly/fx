// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace ScriptingServicesTests
{
    internal sealed class Context1(int baseValue) : IContext1
    {
        private readonly int _baseValue = baseValue;

        public int Sum(int a, int b)
        {
            return _baseValue + a + b;
        }
    }
}
