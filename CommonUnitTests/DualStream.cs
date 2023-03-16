// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using Yextly.Common;

namespace CommonUnitTests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "This is for testing")]
    internal sealed class DualStream : Stream
    {
        private readonly NonOwned<Stream> _actual;
        private readonly NonOwned<Stream> _expected;
        private readonly ITestOutputHelper _testOutputHelper;
        private int _readOperationId;
        private int _writeOperationId;

        public DualStream(Stream expected, Stream actual, ITestOutputHelper testOutputHelper)
        {
            ArgumentNullException.ThrowIfNull(expected);
            ArgumentNullException.ThrowIfNull(actual);
            ArgumentNullException.ThrowIfNull(testOutputHelper);

            _testOutputHelper = testOutputHelper;
            _expected = expected.AsNonOwned();
            _actual = actual.AsNonOwned();
        }

        public override bool CanRead
        {
            get
            {
                var expected = _expected.Value.CanRead;
                var actual = _actual.Value.CanRead;

                Assert.Equal(expected, actual);

                return expected;
            }
        }

        public override bool CanSeek
        {
            get
            {
                var expected = _expected.Value.CanSeek;
                var actual = _actual.Value.CanSeek;

                Assert.Equal(expected, actual);

                return expected;
            }
        }

        public override bool CanWrite
        {
            get
            {
                var expected = _expected.Value.CanWrite;
                var actual = _actual.Value.CanWrite;

                Assert.Equal(expected, actual);

                return expected;
            }
        }

        public override long Length
        {
            get
            {
                long expected;
                long actual;

                try
                {
                    expected = _expected.Value.Length;
                }
                catch (NotSupportedException)
                {
                    expected = -1;
                }

                try
                {
                    actual = _actual.Value.Length;
                }
                catch (NotSupportedException)
                {
                    actual = -1;
                }

                Assert.Equal(expected, actual);

                if (expected < 0)
                    throw new NotSupportedException();
                else
                    return expected;
            }
        }

        public override long Position
        {
            get
            {
                long expected;
                long actual;

                try
                {
                    expected = _expected.Value.Position;
                }
                catch (NotSupportedException)
                {
                    expected = -1;
                }

                try
                {
                    actual = _actual.Value.Position;
                }
                catch (NotSupportedException)
                {
                    actual = -1;
                }

                Assert.Equal(expected, actual);

                if (expected < 0)
                    throw new NotSupportedException();
                else
                    return expected;
            }
            set
            {
                Exception? expectedException = null;
                Exception? actualException = null;

                try
                {
                    _expected.Value.Position = value;
                }
                catch (Exception ex)
                {
                    expectedException = ex;
                }

                try
                {
                    _actual.Value.Position = value;
                }
                catch (Exception ex)
                {
                    actualException = ex;
                }

                if (expectedException != null)
                {
                    Assert.NotNull(actualException);
                    Assert.IsType(expectedException.GetType(), actualException.GetType());

                    throw new NotSupportedException();
                }

                var expectedPosition = _expected.Value.Position;
                var actualPosition = _actual.Value.Position;

                Assert.Equal(expectedPosition, actualPosition);
            }
        }

        public override void Flush()
        {
            _expected.Value.Flush();
            _actual.Value.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var expectedBuffer = new byte[buffer.Length];
            var actualBuffer = new byte[buffer.Length];

            Exception? expectedException = null;
            Exception? actualException = null;

            // here we know insights about the implementation: since we know that actual will be a hybrid stream, we first read from there since the reading operation is paged based
            // and we don't want to read more bytes from expected, since the latter would be misaligned.

            long expected;
            long actual;

            _readOperationId++;
            _testOutputHelper.WriteLine("Begin [" + _readOperationId + "] reading with size " + buffer.Length + " at local offset " + offset + " and count " + count);

            try
            {
                actual = _actual.Value.Read(actualBuffer, offset, count);
                _testOutputHelper.WriteLine("Read " + actual + " bytes: " + string.Join(", ", actualBuffer));
            }
            catch (Exception ex)
            {
                actual = -1;
                actualException = ex;
            }

            try
            {
                int newCount;
                if (actual > 0)
                    newCount = (int)Math.Min(count, actual);
                else
                    newCount = count;

                expected = _expected.Value.Read(expectedBuffer, offset, newCount);
                _testOutputHelper.WriteLine("Read " + expected + " bytes: " + string.Join(", ", expectedBuffer));
            }
            catch (Exception ex)
            {
                expected = -1;
                expectedException = ex;
            }

            if (expectedException != null)
            {
                Assert.NotNull(actualException);
                Assert.IsType(expectedException.GetType(), actualException.GetType());

                throw new NotSupportedException();
            }

            if (expected == 0)
            {
                Assert.Equal(0, actual);
                actualBuffer.CopyTo(buffer, 0);
            }
            else if (expected < 0 || actual < 0)
            {
                Assert.Equal(expected, actual);
            }

            var expectedPosition = _expected.Value.Position;
            var actualPosition = _actual.Value.Position;

            Assert.Equal(expectedPosition, actualPosition);

            var min = (int)Math.Min(expected, actual);

            Assert.Equal(expectedBuffer.AsSpan(0, min).ToArray(), actualBuffer.AsSpan(offset, min).ToArray());

            var source = expected == min ? expectedBuffer : actualBuffer;
            source.AsSpan(0, (int)min).CopyTo(buffer.AsSpan());

            _testOutputHelper.WriteLine("Done reading.");

            return min;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            Exception? expectedException = null;
            Exception? actualException = null;

            long expected;
            long actual;
            try
            {
                expected = _expected.Value.Seek(offset, origin);
            }
            catch (Exception ex)
            {
                expected = -1;
                expectedException = ex;
            }

            try
            {
                actual = _actual.Value.Seek(offset, origin);
            }
            catch (Exception ex)
            {
                actual = -1;
                actualException = ex;
            }

            if (expectedException != null)
            {
                Assert.NotNull(actualException);
                Assert.IsType(expectedException.GetType(), actualException.GetType());

                throw new NotSupportedException();
            }

            Assert.Equal(expected, actual);
            return expected;
        }

        public override void SetLength(long value)
        {
            Exception? expectedException = null;
            Exception? actualException = null;

            long expectedInitialLength = -1;
            long actualInitialLength = -1;

            try
            {
                expectedInitialLength = _expected.Value.Length;
                _expected.Value.SetLength(value);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            try
            {
                actualInitialLength = _actual.Value.Length;
                _actual.Value.SetLength(value);
            }
            catch (Exception ex)
            {
                actualException = ex;
            }

            if (expectedException != null)
            {
                Assert.NotNull(actualException);
                Assert.IsType(expectedException.GetType(), actualException.GetType());

                throw new NotSupportedException();
            }
            var expectedLength = _expected.Value.Length;
            var actualLength = _actual.Value.Length;

            Assert.Equal(expectedLength, actualLength);

            // the following part is to avoid non determinism on the new data
            var count = expectedLength - expectedInitialLength;

            var expectedOriginalPosition = _expected.Value.Position;
            var actualOriginalPosition = _actual.Value.Position;

            Assert.Equal(expectedOriginalPosition, actualOriginalPosition);

            Position = expectedInitialLength;

            var buffer = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };

            while (count > 0)
            {
                var c = Math.Min(buffer.Length, (int)count);
                Write(buffer, 0, c);
                count -= c;
            }

            Position = expectedOriginalPosition;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Exception? expectedException = null;
            Exception? actualException = null;

            _writeOperationId++;
            _testOutputHelper.WriteLine("Begin [" + _writeOperationId + "] writing with size " + buffer.Length + " at local offset " + offset + " and count " + count);
            _testOutputHelper.WriteLine("Write " + count + " bytes starting from offset " + offset + ": " + string.Join(", ", buffer));
            try
            {
                _expected.Value.Write(buffer, offset, count);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            try
            {
                _actual.Value.Write(buffer, offset, count);
            }
            catch (Exception ex)
            {
                actualException = ex;
            }

            if (expectedException != null)
            {
                Assert.NotNull(actualException);
                Assert.IsType(expectedException.GetType(), actualException.GetType());

                throw new NotSupportedException();
            }

            var expectedPosition = _expected.Value.Position;
            var actualPosition = _actual.Value.Position;

            Assert.Equal(expectedPosition, actualPosition);

            var expectedLength = _expected.Value.Length;
            var actualLength = _actual.Value.Length;

            Assert.Equal(expectedLength, actualLength);

            _testOutputHelper.WriteLine("Write done.");
        }

        protected override void Dispose(bool disposing)
        {
            using (_expected)
            {
            }
            using (_actual)
            {
            }

            base.Dispose(disposing);
        }
    }
}
