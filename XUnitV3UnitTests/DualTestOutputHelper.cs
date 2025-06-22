// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Xunit;

namespace XUnitV3UnitTests
{
    internal sealed class DualTestOutputHelper(ITestOutputHelper first, ITestOutputHelper second) : ITestOutputHelper
    {
        private readonly ITestOutputHelper _first = first;
        private readonly ITestOutputHelper _second = second;

        public string Output =>

 // Here we choose to use 'first'
 _first.Output;

        public void Write(string message)
        {
            _first.Write(message);
            _second.Write(message);
        }

        public void Write(string format, params object[] args)
        {
            _first.Write(format, args);
            _second.Write(format, args);
        }

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
