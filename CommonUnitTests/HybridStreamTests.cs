﻿// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Yextly.Common;

namespace CommonUnitTests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "We don't deal with security")]
    public sealed class HybridStreamTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public HybridStreamTests(ITestOutputHelper testOutputHelper)
        {
            ArgumentNullException.ThrowIfNull(testOutputHelper);

            _testOutputHelper = testOutputHelper;
        }

        [InlineData(128, 1, 1, 128, 1)]
        [InlineData(128, 1, 1, 128, 2)]
        [InlineData(128, 1, 1, 129, 1)]
        [InlineData(128, 1, 1, 130, 1)]
        [InlineData(128, 1, 1, 131, 1)]
        [InlineData(128, 1, 1, 132, 1)]
        [InlineData(128, 1, 1, 132, 2)]
        [InlineData(128, 1, 1, 132, 3)]
        [InlineData(128, 1, 1, 132, 4)]
        [InlineData(128, 1, 1, 132, 5)]
        [InlineData(128, 2, 3, 132, 2)]
        [InlineData(128, 2, 3, 133, 2)]
        [InlineData(128, 2, 3, 133, 3)]
        [InlineData(128, 3, 4, 133, 5)]
        [InlineData(117, 3, 2, 151, 4)]
        [InlineData(117, 5, 4, 151, 4)]
        [InlineData(117, 5, 4, 151, 5)]
        [InlineData(117, 5, 5, 151, 5)]
        [InlineData(116, 5, 5, 151, 5)]
        [InlineData(116, 5, 5, 152, 5)]
        [InlineData(116, 5, 5, 152, 6)]
        [InlineData(116, 6, 5, 152, 6)]
        [InlineData(116, 6, 2, 152, 6)]
        [InlineData(116, 6, 3, 152, 6)]
        [InlineData(116, 6, 4, 152, 6)]
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
            CanAppendInternal(length, pageSize, bufferSize, maxLength, ioSize, false, false);
        }

        [InlineData(128, 1, 1, 128, 1)]
        [InlineData(128, 1, 1, 128, 2)]
        [InlineData(128, 1, 1, 129, 1)]
        [InlineData(128, 1, 1, 130, 1)]
        [InlineData(128, 1, 1, 131, 1)]
        [InlineData(128, 1, 1, 132, 1)]
        [InlineData(128, 1, 1, 132, 2)]
        [InlineData(128, 1, 1, 132, 3)]
        [InlineData(128, 1, 1, 132, 4)]
        [InlineData(128, 1, 1, 132, 5)]
        [InlineData(128, 2, 3, 132, 2)]
        [InlineData(128, 2, 3, 133, 2)]
        [InlineData(128, 2, 3, 133, 3)]
        [InlineData(128, 3, 4, 133, 5)]
        [InlineData(117, 3, 2, 151, 4)]
        [InlineData(117, 5, 4, 151, 4)]
        [InlineData(117, 5, 4, 151, 5)]
        [InlineData(117, 5, 5, 151, 5)]
        [InlineData(116, 5, 5, 151, 5)]
        [InlineData(116, 5, 5, 152, 5)]
        [InlineData(116, 5, 5, 152, 6)]
        [InlineData(116, 6, 5, 152, 6)]
        [InlineData(116, 6, 2, 152, 6)]
        [InlineData(116, 6, 3, 152, 6)]
        [InlineData(116, 6, 4, 152, 6)]
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
        public void CanAppendWithMarching(int length, int pageSize, int bufferSize, int maxLength, int ioSize)
        {
            CanAppendInternal(length, pageSize, bufferSize, maxLength, ioSize, false, true);
        }

        [InlineData(128, 1, 1, 128, 1)]
        [InlineData(128, 1, 1, 128, 2)]
        [InlineData(128, 1, 1, 129, 1)]
        [InlineData(128, 1, 1, 130, 1)]
        [InlineData(128, 1, 1, 131, 1)]
        [InlineData(128, 1, 1, 132, 1)]
        [InlineData(128, 1, 1, 132, 2)]
        [InlineData(128, 1, 1, 132, 3)]
        [InlineData(128, 1, 1, 132, 4)]
        [InlineData(128, 1, 1, 132, 5)]
        [InlineData(128, 2, 3, 132, 2)]
        [InlineData(128, 2, 3, 133, 2)]
        [InlineData(128, 2, 3, 133, 3)]
        [InlineData(128, 3, 4, 133, 5)]
        [InlineData(117, 3, 2, 151, 4)]
        [InlineData(117, 5, 4, 151, 4)]
        [InlineData(117, 5, 4, 151, 5)]
        [InlineData(117, 5, 5, 151, 5)]
        [InlineData(116, 5, 5, 151, 5)]
        [InlineData(116, 5, 5, 152, 5)]
        [InlineData(116, 5, 5, 152, 6)]
        [InlineData(116, 6, 5, 152, 6)]
        [InlineData(116, 6, 2, 152, 6)]
        [InlineData(116, 6, 3, 152, 6)]
        [InlineData(116, 6, 4, 152, 6)]
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
        public void CanAppendWithStreamExpansion(int length, int pageSize, int bufferSize, int maxLength, int ioSize)
        {
            CanAppendInternal(length, pageSize, bufferSize, maxLength, ioSize, true, false);
        }

        [InlineData(128, 1, 1, 128, 1)]
        [InlineData(128, 1, 1, 128, 2)]
        [InlineData(128, 1, 1, 129, 1)]
        [InlineData(128, 1, 1, 130, 1)]
        [InlineData(128, 1, 1, 131, 1)]
        [InlineData(128, 1, 1, 132, 1)]
        [InlineData(128, 1, 1, 132, 2)]
        [InlineData(128, 1, 1, 132, 3)]
        [InlineData(128, 1, 1, 132, 4)]
        [InlineData(128, 1, 1, 132, 5)]
        [InlineData(128, 2, 3, 132, 2)]
        [InlineData(128, 2, 3, 133, 2)]
        [InlineData(128, 2, 3, 133, 3)]
        [InlineData(128, 3, 4, 133, 5)]
        [InlineData(117, 3, 2, 151, 4)]
        [InlineData(117, 5, 4, 151, 4)]
        [InlineData(117, 5, 4, 151, 5)]
        [InlineData(117, 5, 5, 151, 5)]
        [InlineData(116, 5, 5, 151, 5)]
        [InlineData(116, 5, 5, 152, 5)]
        [InlineData(116, 5, 5, 152, 6)]
        [InlineData(116, 6, 5, 152, 6)]
        [InlineData(116, 6, 2, 152, 6)]
        [InlineData(116, 6, 3, 152, 6)]
        [InlineData(116, 6, 4, 152, 6)]
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
        public void CanAppendWithStreamExpansionAndMarching(int length, int pageSize, int bufferSize, int maxLength, int ioSize)
        {
            CanAppendInternal(length, pageSize, bufferSize, maxLength, ioSize, true, true);
        }

        [InlineData(8, 3, 5)]
        [InlineData(14, 3, 5)]
        [InlineData(128, 3, 5)]
        [InlineData(128, 3, 2)]
        [InlineData(128, 1, 1)]
        [InlineData(128, 1, 8)]
        [InlineData(128, 2, 8)]
        [InlineData(128, 3, 8)]
        [InlineData(128, 4, 8)]
        [InlineData(128, 5, 8)]
        [InlineData(128, 6, 8)]
        [InlineData(128, 7, 8)]
        [InlineData(128, 8, 8)]
        [InlineData(128, 9, 8)]
        [Theory]
        public void CanEnumerateFromMemoryWithoutWriting(int length, int pageSize, int bufferSize)
        {
            using var expected = CreateStream(length, 0xcc);
            using var clone = CreateStream(length, 0xcc);
            using var actual = new HybridStream(clone, pageSize);

            actual.FetchAllFromStream();

            using var dual = new DualStream(expected, actual, _testOutputHelper);

            Assert.Equal(length, dual.Length);
            Assert.Equal(0, dual.Position);

            var buffer = new byte[bufferSize];
            while (true)
            {
                var r = dual.Read(buffer, 0, buffer.Length);
                if (r == 0)
                {
                    break;
                }
            }
        }

        [InlineData(8, 3, 5)]
        [InlineData(14, 3, 5)]
        [InlineData(128, 3, 5)]
        [InlineData(128, 3, 2)]
        [InlineData(128, 1, 1)]
        [InlineData(128, 1, 8)]
        [InlineData(128, 2, 8)]
        [InlineData(128, 3, 8)]
        [InlineData(128, 4, 8)]
        [InlineData(128, 5, 8)]
        [InlineData(128, 6, 8)]
        [InlineData(128, 7, 8)]
        [InlineData(128, 8, 8)]
        [InlineData(128, 9, 8)]
        [Theory]
        public void CanEnumerateWithoutWriting(int length, int pageSize, int bufferSize)
        {
            using var expected = CreateStream(length, 0xcc);
            using var clone = CreateStream(length, 0xcc);
            using var actual = new HybridStream(clone, pageSize);

            using var dual = new DualStream(expected, actual, _testOutputHelper);

            Assert.Equal(length, dual.Length);
            Assert.Equal(0, dual.Position);

            var buffer = new byte[bufferSize];
            while (true)
            {
                var r = dual.Read(buffer, 0, buffer.Length);
                if (r == 0)
                {
                    break;
                }
            }
        }

        [InlineData(64, 1, 4)]
        [InlineData(64, 1, 0)]
        [InlineData(64, 1, 63)]
        [InlineData(64, 1, 64)]
        [InlineData(64, 3, 4)]
        [InlineData(64, 3, 0)]
        [InlineData(64, 3, 63)]
        [InlineData(64, 3, 64)]
        [InlineData(64, 2, 4)]
        [InlineData(64, 2, 0)]
        [InlineData(64, 2, 63)]
        [InlineData(64, 2, 64)]
        [InlineData(64, 4, 4)]
        [InlineData(64, 4, 0)]
        [InlineData(64, 4, 63)]
        [InlineData(64, 4, 64)]
        [InlineData(64, 8, 4)]
        [InlineData(64, 8, 0)]
        [InlineData(64, 8, 63)]
        [InlineData(64, 8, 64)]
        [InlineData(64, 64, 4)]
        [InlineData(64, 64, 0)]
        [InlineData(64, 64, 63)]
        [InlineData(64, 64, 64)]
        [Theory]
        public void CanModifyASingleValue(int length, int pageSize, int offset)
        {
            using var expected = CreateStream(length, 0xcc, writable: true);
            using var clone = CreateStream(length, 0xcc);
            using var actual = new HybridStream(clone, pageSize);

            using var dual = new DualStream(expected, actual, _testOutputHelper);

            dual.Seek(offset, SeekOrigin.Begin);
            dual.WriteByte(0xff);

            var t = new byte[length];
            _ = dual.Read(t.AsSpan());
        }

        [InlineData(24, 24, 128, 60)]
        [InlineData(12, 24, 128, 60)]
        [InlineData(13, 24, 128, 60)]
        [InlineData(13, 22, 128, 60)]
        [InlineData(13, 21, 128, 60)]
        [InlineData(13, 128, 128, 60)]
        [InlineData(13, 128, 64, 60)]
        [InlineData(13, 128, 32, 60)]
        [InlineData(13, 128, 16, 60)]
        [InlineData(13, 128, 9, 60)]
        [InlineData(13, 128, 6, 60)]
        [InlineData(13, 128, 3, 60)]
        [InlineData(13, 20, 3, 9)]
        [InlineData(13, 20, 2, 9)]
        [InlineData(13, 20, 6, 6)]
        [InlineData(13, 21, 6, 6)]
        [InlineData(13, 21, 3, 6)]
        [Theory]
        public void CanPerformBulkOperations(int initialLength, int finalLength, int pageSize, int operations)
        {
            var random = CreateRandom();

            using var expected = CreateStream(initialLength, 0xcc, writable: true);
            using var clone = CreateStream(initialLength, 0xcc);
            using var actual = new HybridStream(clone, pageSize);

            using var dual = new DualStream(expected, actual, _testOutputHelper);

            var array = new byte[finalLength];
            random.NextBytes(array);

            if (finalLength > initialLength)
            {
                dual.SetLength(finalLength);
            }

            for (int i = 0; i < operations; i++)
            {
                var offset = random.Next(finalLength - 1);
                var count = 1 + random.Next(finalLength - offset);

                dual.Write(array, offset, count);
            }

            dual.Seek(0, SeekOrigin.Begin);

            var t = new byte[finalLength];
            _ = dual.Read(t.AsSpan());
        }

        [InlineData(15310, 2451, 3)]
        [InlineData(15310, 2451, 2)]
        [InlineData(15310, 2451, 1)]
        [InlineData(15310, 2451, 0)]
        [Theory]
        public void CanReadTheStreamMultipleTimes(int initialSize, int additionalSize, int times)
        {
            var buffer = new byte[initialSize];

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(i % 255);
            }

            using var memoryStream = new MemoryStream(buffer, false);

            using var stream = new HybridStream(memoryStream);

            WriteSequentialBytes(stream, initialSize, additionalSize);

            for (int i = 0; i < times; i++)
            {
                stream.Seek(0, SeekOrigin.Begin);
                Assert.Equal(0, stream.Position);

                AssertSequential(stream, initialSize + additionalSize);
            }
        }

        [InlineData(15310, 2451, 3)]
        [InlineData(15310, 2451, 2)]
        [InlineData(15310, 2451, 1)]
        [InlineData(15310, 2451, 0)]
        [Theory]
        public async Task CanReadTheStreamMultipleTimesAsync(int initialSize, int additionalSize, int times)
        {
            var buffer = new byte[initialSize];

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(i % 255);
            }

            using var memoryStream = new MemoryStream(buffer, false);

            using var stream = new HybridStream(memoryStream);

            await WriteSequentialBytesAsync(stream, initialSize, additionalSize).ConfigureAwait(true);

            for (int i = 0; i < times; i++)
            {
                stream.Seek(0, SeekOrigin.Begin);
                Assert.Equal(0, stream.Position);

                await AssertSequentialAsync(stream, initialSize + additionalSize).ConfigureAwait(true);
            }
        }

        [Fact]
        public void CanReadWithZeroBuffer()
        {
            const int length = 61_046_536;
            const int pageSize = 16_384;
            const int newLength = 3_726 * pageSize;

            using var inner = CreateStream(length, 0xcc);
            using var actual = new HybridStream(inner, pageSize);

            actual.SetLength(newLength);
            actual.Seek(0, SeekOrigin.Begin);
            var read = actual.Read([], 0, 0);

            Assert.Equal(0, read);
        }

        [Fact]
        public void CanReadWithZeroCount()
        {
            const int length = 61_046_536;
            const int pageSize = 16_384;
            const int newLength = 3_726 * pageSize;

            using var inner = CreateStream(length, 0xcc);
            using var actual = new HybridStream(inner, pageSize);

            actual.SetLength(newLength);
            actual.Seek(0, SeekOrigin.Begin);
            var read = actual.Read(new byte[16], 0, 0);

            Assert.Equal(0, read);
        }

        [InlineData(30, 6, 30, 3)]
        [InlineData(30, 2, 27, 3)]
        [InlineData(30, 1, 30, 2)]
        [InlineData(30, 1, 30, 1)]
        [InlineData(30, 1, 30, 3)]
        [InlineData(63, 31, 64, 3)]
        [InlineData(64, 32, 64, 4)]
        [InlineData(64, 32, 64, 1)]
        [InlineData(64, 32, 64, 3)]
        [Theory]
        public void CanTruncateAndExtendTheStream(int initialLength, int intermediateLength, int finalLength, int pageSize)
        {
            using var expected = CreateStream(initialLength, 0xcc, writable: true);
            using var clone = CreateStream(initialLength, 0xcc);
            using var actual = new HybridStream(clone, pageSize);

            using var dual = new DualStream(expected, actual, _testOutputHelper);

            dual.SetLength(intermediateLength);
            dual.SetLength(finalLength);

            dual.Seek(0, SeekOrigin.Begin);

            var t = new byte[finalLength];
            _ = dual.Read(t.AsSpan());
        }

        [InlineData(26, 20, 3)]
        [InlineData(26, 20, 4)]
        [InlineData(23, 20, 5)]
        [InlineData(26, 20, 5)]
        [InlineData(25, 20, 5)]
        [InlineData(63, 31, 1)]
        [InlineData(63, 31, 2)]
        [InlineData(63, 31, 3)]
        [InlineData(63, 31, 4)]
        [InlineData(63, 32, 2)]
        [InlineData(63, 32, 3)]
        [InlineData(63, 32, 4)]
        [InlineData(64, 32, 4)]
        [InlineData(64, 32, 1)]
        [InlineData(64, 32, 3)]
        [Theory]
        public void CanTruncateTheStream(int initialLength, int finalLength, int pageSize)
        {
            using var expected = CreateStream(initialLength, 0xcc, writable: true);
            using var clone = CreateStream(initialLength, 0xcc);
            using var actual = new HybridStream(clone, pageSize);

            using var dual = new DualStream(expected, actual, _testOutputHelper);

            dual.SetLength(finalLength);

            dual.Seek(0, SeekOrigin.Begin);

            var t = new byte[finalLength];
            _ = dual.Read(t.AsSpan());
        }

        [Fact]
        public void CanWriteAtTheBoundaries()
        {
            const int length = 61_046_536;
            const int pageSize = 16_384;
            const int newLength = (3_726 * pageSize) - 1;

            const int expectedLength = newLength + 1;

            using var inner = CreateStream(length, 0xcc);
            using var actual = new HybridStream(inner, pageSize);

            actual.SetLength(newLength);
            actual.Seek(0, SeekOrigin.End);

            // workaround do defeat the analyzer that thinks we are writing a string
            var array = new List<byte> { 0x77 };
            actual.Write([.. array], 0, 1);

            Assert.Equal(expectedLength, actual.Length);
        }

        [Fact]
        public void CanWriteAtTheBoundariesWithZeroCount()
        {
            const int length = 61_046_536;
            const int pageSize = 16_384;
            const int newLength = 3_726 * pageSize;

            using var inner = CreateStream(length, 0xcc);
            using var actual = new HybridStream(inner, pageSize);

            actual.SetLength(newLength);
            actual.Seek(0, SeekOrigin.End);
            actual.Write([], 0, 0);

            Assert.Equal(newLength, actual.Length);
        }

        private static void AssertSequential(Stream source, int expectedLength)
        {
            int count = 0;

            int b;
            while ((b = source.ReadByte()) >= 0)
            {
                var expected = (byte)(count % 255);
                var actual = (byte)b;

                Assert.Equal(expected, actual);

                count++;
            }

            Assert.Equal(expectedLength, count);
        }

        private static async Task AssertSequentialAsync(Stream source, int expectedLength)
        {
            int count = 0;

            var buffer = new byte[1];

            while ((await source.ReadAsync(buffer).ConfigureAwait(false)) > 0)
            {
                var expected = (byte)(count % 255);
                var actual = buffer[0];

                Assert.Equal(expected, actual);

                count++;
            }

            Assert.Equal(expectedLength, count);
        }

        private static Random CreateRandom()
        {
            return new(0);
        }

        private static MemoryStream CreateStream(int length, byte seed = 0, bool writable = false)
        {
            var buffer = new byte[length];

            var s = seed;
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = s;
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
            {
                return new MemoryStream(buffer, false);
            }
        }

        private static void WriteSequentialBytes(Stream source, int currentLength, int bytesToAdd)
        {
            var finalLength = currentLength + bytesToAdd;
            var buffer = new byte[1];

            source.Seek(0, SeekOrigin.End);

            for (int i = currentLength; i < finalLength; i++)
            {
                buffer[0] = (byte)(i % 255);
                source.Write(buffer);
            }

            source.Flush();
        }

        private static async Task WriteSequentialBytesAsync(Stream source, int currentLength, int bytesToAdd)
        {
            var finalLength = currentLength + bytesToAdd;
            var buffer = new byte[1];

            source.Seek(0, SeekOrigin.End);

            for (int i = currentLength; i < finalLength; i++)
            {
                buffer[0] = (byte)(i % 255);
                await source.WriteAsync(buffer).ConfigureAwait(false);
            }

            await source.FlushAsync().ConfigureAwait(false);
        }

        private void CanAppendInternal(int length, int pageSize, int bufferSize, int maxLength, int ioSize, bool setInitialLength, bool march)
        {
            using var expected = CreateStream(length, 0xcc, writable: true);
            using var clone = CreateStream(length, 0xcc);
            using var actual = new HybridStream(clone, pageSize);

            using var dual = new DualStream(expected, actual, _testOutputHelper);

            Assert.Equal(length, dual.Length);
            Assert.Equal(0, dual.Position);

            var random = HybridStreamTests.CreateRandom();

            if (setInitialLength)
            {
                dual.SetLength(maxLength);
            }

            dual.Seek(0, SeekOrigin.End);

            var count = maxLength - length;

            var array = new byte[count];
            random.NextBytes(array);

            var buffer = new byte[bufferSize];

            _testOutputHelper.WriteLine("Writing...");

            int index = 0;
            while (count > 0)
            {
                var c = (ioSize < 0) ? random.Next(count) + 1 : Math.Min(ioSize, count);
                dual.Write(array, index, c);
                index += c;
                count -= c;

                if (march)
                {
                    _testOutputHelper.WriteLine("Marching back...");
                    dual.Seek(0, SeekOrigin.Begin);

                    var t = new byte[maxLength];
                    _ = dual.Read(t.AsSpan());

                    dual.Seek(0, SeekOrigin.End);
                }
            }

            _testOutputHelper.WriteLine("Reading forward...");
            for (int i = 0; i < maxLength; i++)
            {
                dual.Seek(0, SeekOrigin.Begin);

                while (true)
                {
                    var r = dual.Read(buffer, 0, buffer.Length);
                    if (r == 0)
                    {
                        break;
                    }
                }
            }

            _testOutputHelper.WriteLine("Reading backward...");
            for (int i = length - 1; i >= 0; i--)
            {
                dual.Seek(0, SeekOrigin.End);

                while (true)
                {
                    var r = dual.Read(buffer, 0, buffer.Length);
                    if (r == 0)
                    {
                        break;
                    }
                }
            }
        }
    }
}
