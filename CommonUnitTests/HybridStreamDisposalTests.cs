// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Moq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Yextly.Common;

namespace CommonUnitTests
{
    public sealed class HybridStreamDisposalTests : IDisposable
    {
        private readonly MemoryStream _memoryStream;
        private readonly HybridStream _stream;

        public HybridStreamDisposalTests()
        {
            _memoryStream = new();
            _memoryStream.WriteByte(0);
            _memoryStream.WriteByte(1);
            _memoryStream.WriteByte(2);
            _memoryStream.WriteByte(3);

            _stream = new(_memoryStream);
            _stream.Seek(0, SeekOrigin.End);

            ((IDisposable)_stream).Dispose();
        }

        [Fact]
        public void BeginRead()
        {
            Assert.ThrowsAny<NotSupportedException>(() => _ = _stream.BeginRead(new byte[10], 0, 2, null, null));
        }

        [Fact]
        public void BeginWrite()
        {
            Assert.ThrowsAny<NotSupportedException>(() => _ = _stream.BeginWrite(new byte[10], 0, 2, null, null));
        }

        [Fact]
        public void CanRead()
        {
            var actual = _stream.CanRead;

            Assert.False(actual);
        }

        [Fact]
        public void CanSeek()
        {
            var actual = _stream.CanSeek;

            Assert.False(actual);
        }

        [Fact]
        public void CanTimeout()
        {
            var actual = _stream.CanTimeout;

            Assert.False(actual);
        }

        [Fact]
        public void CanWrite()
        {
            var actual = _stream.CanWrite;

            Assert.False(actual);
        }

        [Fact]
        public void Close()
        {
            _stream.Close();
            Assert.NotNull(_stream);
        }

        [Fact]
        public void CopyTo1()
        {
            using var stream = new MemoryStream();
            Assert.ThrowsAny<ObjectDisposedException>(() => _stream.CopyTo(stream));
        }

        [Fact]
        public void CopyTo2()
        {
            using var stream = new MemoryStream();
            Assert.ThrowsAny<ObjectDisposedException>(() => _stream.CopyTo(stream, 10));
        }

        [Fact]
        public async Task CopyToAsync1()
        {
            await using var disposableStream = new MemoryStream().AsAsyncDisposable(out var stream).ConfigureAwait(true);
            await Assert.ThrowsAnyAsync<ObjectDisposedException>(async () => await _stream.CopyToAsync(stream).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task CopyToAsync2()
        {
            await using var disposableStream = new MemoryStream().AsAsyncDisposable(out var stream).ConfigureAwait(true);
            await Assert.ThrowsAnyAsync<ObjectDisposedException>(async () => await _stream.CopyToAsync(stream, 10).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task CopyToAsync3()
        {
            await using var disposableStream = new MemoryStream().AsAsyncDisposable(out var stream).ConfigureAwait(true);
            await Assert.ThrowsAnyAsync<ObjectDisposedException>(async () => await _stream.CopyToAsync(stream, default(CancellationToken)).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task CopyToAsync4()
        {
            await using var disposableStream = new MemoryStream().AsAsyncDisposable(out var stream).ConfigureAwait(true);
            await Assert.ThrowsAnyAsync<ObjectDisposedException>(async () => await _stream.CopyToAsync(stream, 10, default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        public void Dispose()
        {
            using (_memoryStream)
            {
                using (_stream)
                {
                    // Nothing here on purpose
                }
            }
        }

        [Fact]
        public async Task DisposeAsync()
        {
            await _stream.DisposeAsync().ConfigureAwait(true);

            Assert.NotNull(_stream);
        }

        [Fact]
        public void EndRead()
        {
            var result = new Mock<IAsyncResult>();

            // Note that this is not really a test, since we should offer a real AsyncResult instance,
            // however, that would call the internal read methods, therefore the effect wuld be the same.
            // The test here to signal that we have not forgotten any entries to test.
            Assert.ThrowsAny<ArgumentException>(() => _stream.EndRead(result.Object));
        }

        [Fact]
        public void EndWrite()
        {
            var result = new Mock<IAsyncResult>();

            // Note that this is not really a test, since we should offer a real AsyncResult instance,
            // however, that would call the internal read methods, therefore the effect wuld be the same.
            // The test here to signal that we have not forgotten any entries to test.
            Assert.ThrowsAny<ArgumentException>(() => _stream.EndWrite(result.Object));
        }

        [Fact]
        public void Flush()
        {
            using var stream = new MemoryStream();
            Assert.ThrowsAny<ObjectDisposedException>(() => _stream.Flush());
        }

        [Fact]
        public async Task FlushAsync1()
        {
            await using var disposableStream = new MemoryStream().AsAsyncDisposable(out var stream).ConfigureAwait(true);
            Assert.NotNull(stream);
            await Assert.ThrowsAnyAsync<ObjectDisposedException>(async () => await _stream.FlushAsync().ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task FlushAsync2()
        {
            await using var disposableStream = new MemoryStream().AsAsyncDisposable(out var stream).ConfigureAwait(true);
            Assert.NotNull(stream);
            await Assert.ThrowsAnyAsync<ObjectDisposedException>(async () => await _stream.FlushAsync(default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public void Length()
        {
            var actual = _stream.Length;

            Assert.Equal(0, actual);
        }

        [Fact]
        public void Position1()
        {
            var actual = _stream.Position;

            Assert.Equal(0, actual);
        }

        [Fact]
        public void Position2()
        {
            Assert.ThrowsAny<ObjectDisposedException>(() => _stream.Position = 0);
        }

        [Fact]
        public void Read1()
        {
            Assert.ThrowsAny<ObjectDisposedException>(() => _ = _stream.Read(new byte[10], 0, 1));
        }

        [Fact]
        public void Read2()
        {
            Assert.ThrowsAny<ObjectDisposedException>(() => _ = _stream.Read(new byte[10].AsSpan()));
        }

        [Fact]
        public async Task ReadAsync1()
        {
            await using var disposableStream = new MemoryStream().AsAsyncDisposable(out var stream).ConfigureAwait(true);
            Assert.NotNull(stream);
            await Assert.ThrowsAnyAsync<NotSupportedException>(async () => _ = await _stream.ReadAsync((new byte[10]).AsMemory()).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1835:Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'", Justification = "We need to test all methods here")]
        public async Task ReadAsync2()
        {
            await using var disposableStream = new MemoryStream().AsAsyncDisposable(out var stream).ConfigureAwait(true);
            Assert.NotNull(stream);
            await Assert.ThrowsAnyAsync<NotSupportedException>(async () => _ = await _stream.ReadAsync(new byte[10], 2, 1).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1835:Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'", Justification = "We need to test all methods here")]
        public async Task ReadAsync3()
        {
            await using var disposableStream = new MemoryStream().AsAsyncDisposable(out var stream).ConfigureAwait(true);
            Assert.NotNull(stream);
            await Assert.ThrowsAnyAsync<NotSupportedException>(async () => _ = await _stream.ReadAsync(new byte[10], 2, 1, default).ConfigureAwait(true)).ConfigureAwait(true);
        }

#if NET8_0_OR_GREATER
        [Fact]
        public void ReadAtLeast()
        {
            Assert.ThrowsAny<ObjectDisposedException>(() => _ = _stream.ReadAtLeast(new byte[10].AsSpan(), 4));
        }

        [Fact]
        public async Task ReadAtLeastAsync()
        {
            await using var disposableStream = new MemoryStream().AsAsyncDisposable(out var stream).ConfigureAwait(true);
            Assert.NotNull(stream);
            await Assert.ThrowsAnyAsync<NotSupportedException>(async () => _ = await _stream.ReadAtLeastAsync(new byte[10].AsMemory(), 10).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task ReadAtLeastAsync1()
        {
            await using var disposableStream = new MemoryStream().AsAsyncDisposable(out var stream).ConfigureAwait(true);
            Assert.NotNull(stream);
            await Assert.ThrowsAnyAsync<NotSupportedException>(async () => await _stream.ReadExactlyAsync(new byte[10].AsMemory()).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task ReadAtLeastAsync2()
        {
            await using var disposableStream = new MemoryStream().AsAsyncDisposable(out var stream).ConfigureAwait(true);
            Assert.NotNull(stream);
            await Assert.ThrowsAnyAsync<NotSupportedException>(async () => await _stream.ReadExactlyAsync(new byte[10], 0, 10).ConfigureAwait(true)).ConfigureAwait(true);
        }
#endif

        [Fact]
        public void ReadByte()
        {
            Assert.ThrowsAny<ObjectDisposedException>(() => _ = _stream.ReadByte());
        }

#if NET8_0_OR_GREATER
        [Fact]
        public void ReadExactly()
        {
            Assert.ThrowsAny<ObjectDisposedException>(() => _stream.ReadExactly(new byte[10].AsSpan()));
        }
#endif

        [Fact]
        public void ReadTimeout()
        {
            Assert.ThrowsAny<InvalidOperationException>(() => _ = _stream.ReadTimeout);
        }

        [Fact]
        public void Seek()
        {
            Assert.ThrowsAny<ObjectDisposedException>(() => _ = _stream.Seek(0, SeekOrigin.Begin));
        }

        [Fact]
        public void SetLength()
        {
            Assert.ThrowsAny<ObjectDisposedException>(() => _stream.SetLength(0));
        }

        [Fact]
        public void Write1()
        {
            Assert.ThrowsAny<ObjectDisposedException>(() => _stream.Write(new byte[10], 0, 10));
        }

        [Fact]
        public void Write2()
        {
            Assert.ThrowsAny<ObjectDisposedException>(() => _stream.Write(new byte[10].AsSpan()));
        }

        [Fact]
        public async Task WriteAsync1()
        {
            await using var disposableStream = new MemoryStream().AsAsyncDisposable(out var stream).ConfigureAwait(true);
            Assert.NotNull(stream);
            await Assert.ThrowsAnyAsync<NotSupportedException>(async () => await _stream.WriteAsync(new byte[10].AsMemory()).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1835:Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'", Justification = "We need to test all methods here")]
        public async Task WriteAsync2()
        {
            await using var disposableStream = new MemoryStream().AsAsyncDisposable(out var stream).ConfigureAwait(true);
            Assert.NotNull(stream);
            await Assert.ThrowsAnyAsync<NotSupportedException>(async () => await _stream.WriteAsync(new byte[10], 0, 5).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1835:Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'", Justification = "We need to test all methods here")]
        public async Task WriteAsync3()
        {
            await using var disposableStream = new MemoryStream().AsAsyncDisposable(out var stream).ConfigureAwait(true);
            Assert.NotNull(stream);
            await Assert.ThrowsAnyAsync<NotSupportedException>(async () => await _stream.WriteAsync(new byte[10], 0, 5, default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public void WriteByte()
        {
            Assert.ThrowsAny<ObjectDisposedException>(() => _stream.WriteByte(0));
        }

        [Fact]
        public void WriteTimeout()
        {
            Assert.ThrowsAny<InvalidOperationException>(() => _ = _stream.WriteTimeout);
        }
    }
}
