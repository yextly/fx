// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Xunit.Abstractions;

namespace XUnitUnitTests
{
    internal sealed class DualTestOutputHelper(ITestOutputHelper first, ITestOutputHelper second) : ITestOutputHelper
    {
        private readonly ITestOutputHelper _first = first;
        private readonly ITestOutputHelper _second = second;

        public void WriteLine(string message)
        {
            _first.WriteLine(message);
            _second.WriteLine(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            _first.WriteLine(format, args);
            _second.WriteLine(format, args);
        }
    }
}
