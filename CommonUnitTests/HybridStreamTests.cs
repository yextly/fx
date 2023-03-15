// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System;
using System.IO;
using Xunit;
using Yextly.Common;

namespace CommonUnitTests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "We don't deal with security")]
    public sealed class HybridStreamTests
    {
        [InlineData(128, 3, 4, 129, 5)]
        [InlineData(128, 3, 4, 131, 5)]
        [InlineData(128, 3, 3, 131, 5)]
        [InlineData(128, 2, 3, 131, 5)]
        [InlineData(128, 2, 3, 131, 4)]
        [InlineData(128, 2, 3, 131, 3)]
        [InlineData(128, 2, 3, 131, 2)]
        [InlineData(128, 2, 3, 131, 1)]
        [InlineData(128, 1, 3, 129, 3)]
        [InlineData(128, 1, 4, 132, 2)]
        [InlineData(128, 1, 4, 131, 2)]
        [InlineData(128, 1, 4, 131, 4)]
        [InlineData(128, 1, 4, 131, 3)]
        [InlineData(128, 1, 3, 131, 3)]
        [InlineData(128, 1, 2, 131, 3)]
        [InlineData(128, 1, 1, 131, 3)]
        [Theory]
        public void CanAppend(int length, int pageSize, int bufferSize, int maxLength, int ioSize)
        {
            using var expected = CreateStream(length, 0xcc, writable: true);
            using var clone = CreateStream(length, 0xcc);
            using var actual = new HybridStream(clone, pageSize);

            using var dual = new DualStream(expected, actual);

            Assert.Equal(length, dual.Length);
            Assert.Equal(0, dual.Position);

            var random = HybridStreamTests.CreateRandom();

            dual.Seek(0, SeekOrigin.End);

            var count = maxLength - length;

            var array = new byte[count];
            random.NextBytes(array);

            int index = 0;
            while (count > 0)
            {
                var c = (ioSize < 0) ? random.Next(count) + 1 : Math.Min(ioSize, count);
                dual.Write(array, index, c);
                index += c;
                count -= c;
            }

            var buffer = new byte[bufferSize];
            while (true)
            {
                var r = dual.Read(buffer, 0, buffer.Length);
                if (r == 0)
                    break;
            }
        }

        [InlineData(128, 3, 5)]
        [InlineData(128, 3, 2)]
        [InlineData(128, 1, 1)]
        [InlineData(128, 1, 8)]
        [Theory]
        public void CanEnumerateWithoutWriting(int length, int pageSize, int bufferSize)
        {
            using var expected = CreateStream(length, 0xcc);
            using var clone = CreateStream(length, 0xcc);
            using var actual = new HybridStream(clone, pageSize);

            using var dual = new DualStream(expected, actual);

            Assert.Equal(length, dual.Length);
            Assert.Equal(0, dual.Position);

            var buffer = new byte[bufferSize];
            while (true)
            {
                var r = dual.Read(buffer, 0, buffer.Length);
                if (r == 0)
                    break;
            }
        }

        private static Random CreateRandom() => new(0);

        private static Stream CreateStream(int length, byte seed = 0, bool writable = false)
        {
            var buffer = new byte[length];

            var s = seed;
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)s;
                s = (byte)(((int)s + 1) % 255);
            }

            if (writable)
            {
                var stream = new MemoryStream();
                stream.Write(buffer, 0, buffer.Length);
                stream.Seek(0, SeekOrigin.Begin);

                return stream;
            }
            else
                return new MemoryStream(buffer, false);
        }
    }
}
