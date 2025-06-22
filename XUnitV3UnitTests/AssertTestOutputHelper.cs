// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace XUnitV3UnitTests
{
    internal sealed class AssertTestOutputHelper : ITestOutputHelper
    {
        private string _current = string.Empty;

        public string Output => throw new System.NotSupportedException();

        public async Task<string> GetValueAsync(CancellationToken cancellationToken)
        {
            string actual;

            while (true)
            {
                actual = Volatile.Read(ref _current);
                if (!string.IsNullOrEmpty(actual))
                {
                    return actual;
                }

                await Task.Delay(100, cancellationToken).ConfigureAwait(false);
            }
        }

        public void Write(string message)
        {
            Interlocked.Exchange(ref _current, message);
        }

        public void Write(string format, params object[] args)
        {
            WriteLine(format, args);
        }

        public void WriteLine(string message)
        {
            Interlocked.Exchange(ref _current, message);
        }

        public void WriteLine(string format, params object[] args)
        {
            var t = string.Format(CultureInfo.InvariantCulture, format, args);
            Interlocked.Exchange(ref _current, t);
        }
    }
}
