// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Buffers;
using System.Diagnostics;

namespace Yextly.Common
{
    /// <summary>
    /// Represents a stream that performs writes in memory only using copy-on-write buffers (pages).
    /// </summary>
    public sealed class HybridStream : Stream
    {
        private const int DefaultPageSize = 4096;
        private readonly NonOwned<Stream> _inner;
        private readonly long _originalLength;
        private readonly List<byte[]?> _pages;
        private readonly int _pageSize;
        private long _currentLength;
        private long _currentPosition;

        /// <summary>
        /// Creates a new <see cref="HybridStream"/> from an existing source.
        /// </summary>
        /// <param name="source">An existing source providing the read-only data.</param>
        public HybridStream(Stream source) : this(source, DefaultPageSize)
        {
        }

        /// <summary>
        /// Creates a new <see cref="HybridStream"/> from an existing source.
        /// </summary>
        /// <param name="source">An existing source providing the read-only data.</param>
        /// <param name="pageSize">Specifies the size of the memory pages used for CoW operations.</param>
        public HybridStream(Stream source, int pageSize)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentOutOfRangeThrowHelper.ThrowIfNegativeOrZero(pageSize);

            _inner = source.AsNonOwned();
            _pageSize = pageSize;

            if (!_inner.Value.CanSeek)
                throw new InvalidOperationException("Only Seekable streams are supported.");

            _originalLength = _inner.Value.Length;
            _currentLength = _originalLength;
            _pages = [];
            _currentPosition = _inner.Value.Position;

            EnsureAvailablePagesFor(_currentLength, false);
        }

        /// <inheritdoc/>
        public override bool CanRead => _inner.Value.CanRead;

        /// <inheritdoc/>
        public override bool CanSeek => _inner.Value.CanSeek;

        /// <inheritdoc/>
        public override bool CanWrite => true;

        /// <inheritdoc/>
        public override long Length => _currentLength;

        /// <inheritdoc/>
        public override long Position { get => _currentPosition; set => SeekInternal(value, SeekOrigin.Begin); }

        /// <inheritdoc/>
        public override void Flush()
        {
            _inner.Value.Flush();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            ArgumentNullException.ThrowIfNull(buffer);

            var sourceOffset = _currentPosition;
            var endOffset = _currentPosition + count;

            if (sourceOffset > endOffset)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (endOffset > _currentLength)
                endOffset = _currentLength;

            if (buffer.Length == 0 || sourceOffset == endOffset)
                return 0;

            var sourcePageIndex = GetPageIndex(sourceOffset, out var firstPageStartOffset);
            var endPageIndex = GetPageIndex(endOffset, out var lastPageEndOffset);

            var destinationOffset = offset;
            var copiedBytes = 0;
            var destinationLastOffset = offset + count;

            int currentPageIndex = sourcePageIndex;
            do
            {
                if (TryGetPage(currentPageIndex, out var page))
                {
                    var pageStartOffset = (currentPageIndex == sourcePageIndex) ? firstPageStartOffset : 0;
                    var pageEndOffset = (currentPageIndex == endPageIndex) ? lastPageEndOffset : _pageSize;

                    var pageSize = pageEndOffset - pageStartOffset;

                    if (pageSize <= 0)
                        break;

                    var bytesToCopy = Math.Min(pageSize, destinationLastOffset - destinationOffset);

                    if (page == null)
                    {
                        Debug.WriteLine("Read from " + _inner.Value.Position + " at worst " + bytesToCopy + " bytes");
                        Debug.WriteLine("Source offset: " + sourceOffset);
                        Debug.WriteLine("End offset: " + endOffset);

                        if (pageStartOffset > 0)
                            _inner.Value.Seek(pageStartOffset, SeekOrigin.Current);

                        var read = _inner.Value.Read(buffer.AsSpan(destinationOffset, bytesToCopy));
                        Debug.WriteLine("Read: " + read);

                        copiedBytes += read;
                        destinationOffset += read;
                        _currentPosition += read;
                    }
                    else
                    {
                        var span = page.AsSpan(pageStartOffset, bytesToCopy);
                        span.CopyTo(buffer.AsSpan(destinationOffset, destinationLastOffset - destinationOffset));
                        copiedBytes += bytesToCopy;
                        destinationOffset += bytesToCopy;
                        _currentPosition += bytesToCopy;
                    }
                }

                currentPageIndex++;
            } while (copiedBytes < count && currentPageIndex <= endPageIndex);

            return copiedBytes;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return SeekInternal(offset, origin);
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            ArgumentOutOfRangeThrowHelper.ThrowIfNegative(value);

            if (value == _currentLength)
            {
                // nothing to do
            }
            else if (value > _currentLength)
            {
                // expansion
                EnsureAvailablePagesFor(value, true);
                _currentLength = value;
            }
            else
            {
                // this is a truncation. Note that truncating the stream has two important side effects:
                // - cow pages are discarded when they exceed the original stream length (optional, at the moment not implemented)
                // - new cow pages are allocated to cover the overlap between the new length and the original one in order to disallow reading the old "deleted" data

                var firstPageIndex = GetPageIndex(value, out _);

                // the first page must be fetched since it could contain partial valid data
                _ = GetCoWPage(firstPageIndex);

                for (int i = firstPageIndex + 1; i < _pages.Count; i++)
                {
                    var p = _pages[i];
                    if (p == null)
                        _pages[i] = AllocateNewDirtyPage();
                }

                _currentLength = value;
            }
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            ArgumentNullException.ThrowIfNull(buffer);

            var sourceOffset = _currentPosition;
            var endOffset = _currentPosition + count;

            if (sourceOffset > endOffset)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (count == 0)
                return;

            EnsureAvailablePagesFor(endOffset, true);

            var sourcePageIndex = GetPageIndex(sourceOffset, out var firstPageStartOffset);
            var endPageIndex = GetPageIndex(endOffset, out var lastPageEndOffset);

            var bufferOffset = offset;
            var totalBytesToCopy = count;

            int currentPageIndex = sourcePageIndex;
            do
            {
                var page = GetCoWPage(currentPageIndex);

                var pageStartOffset = (currentPageIndex == sourcePageIndex) ? firstPageStartOffset : 0;
                var pageEndOffset = (currentPageIndex == endPageIndex) ? lastPageEndOffset + 1 : _pageSize;

                var pageSize = pageEndOffset - pageStartOffset;

                Debug.Assert(pageSize >= 0);

                var bytesToCopy = Math.Min(pageSize, totalBytesToCopy);
                if (bytesToCopy == 0)
                {
                    currentPageIndex++;
                }
                else
                {
                    buffer.AsSpan(bufferOffset, bytesToCopy).CopyTo(page.AsSpan(pageStartOffset, pageSize));
                    totalBytesToCopy -= bytesToCopy;
                    bufferOffset += bytesToCopy;
                    _currentPosition += bytesToCopy;
                }

                if (bytesToCopy == pageSize)
                    currentPageIndex++;
            } while (totalBytesToCopy > 0 && currentPageIndex <= endPageIndex);

            // moral equivalent of set length
            if (_currentPosition > _currentLength)
                _currentLength = _currentPosition;
        }

        internal void FetchAllFromStream()
        {
            EnsureAvailablePagesFor(_currentLength, true);

            for (int i = 0; i < _pages.Count; i++)
            {
                _ = GetCoWPage(i);
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            using (_inner)
            {
            }

            foreach (var page in _pages)
            {
                if (page != null)
                    DeallocatePage(page);
            }
            _pages.Clear();

            base.Dispose(disposing);
        }

        private static void DeallocatePage(byte[] page)
        {
            ArrayPool<byte>.Shared.Return(page);
        }

        [Conditional("DEBUG")]
        private static void TagPage(byte[] page)
        {
            var array = new byte[] { 0xcc, 0xdd, 0xcd, 0xdc, 0xdd, 0xcc };
            for (int i = 0; i < page.Length; i++)
            {
                page[i] = array[i % array.Length];
            }
        }

        private byte[] AllocateNewDirtyPage()
        {
            var page = ArrayPool<byte>.Shared.Rent(_pageSize);

            TagPage(page);

            return page;
        }

        private void EnsureAvailablePagesFor(long size, bool fetchFromStream)
        {
            var pageCount = Math.DivRem(size, _pageSize, out var t);

            var offOne = t > 0;

            var count = pageCount + (offOne ? 1 : 0);

            var c = (int)count - _pages.Count;
            if (c > 0)
            {
                _pages.EnsureCapacity((int)count);
                if (fetchFromStream)
                {
                    for (int i = 0; i < c; i++)
                    {
                        var index = _pages.Count;
                        _pages.Add(null);

                        _ = GetCoWPage(index);
                    }
                }
                else
                {
                    _pages.AddRange(Enumerable.Repeat((byte[]?)null, c));
                }
            }
        }

        private Byte[] GetCoWPage(int index)
        {
            var p = _pages[index];
            if (p == null)
            {
                p = AllocateNewDirtyPage();
                _pages[index] = p;
                var offset = index * _pageSize;
                if (offset < _originalLength)
                {
                    _inner.Value.Seek(offset, SeekOrigin.Begin);
                    var count = (int)Math.Min(offset + _pageSize, _originalLength) - offset;

                    var initialOffset = 0;
                    while (count > 0)
                    {
                        var r = _inner.Value.Read(p, initialOffset, count);
                        count -= r;
                        initialOffset += r;
                    }
                }
                return p;
            }
            else
            {
                return p;
            }
        }

        private Byte[]? GetPage(int index)
        {
            var p = _pages[index];
            if (p == null)
            {
                var offset = index * _pageSize;
                Debug.WriteLine("Move position to " + offset);
                _inner.Value.Seek(offset, SeekOrigin.Begin);
            }

            return p;
        }

        private int GetPageIndex(long offset, out int offsetInPage)
        {
            return Math.DivRem((int)offset, _pageSize, out offsetInPage);
        }

        private long SeekInternal(long offset, SeekOrigin origin)
        {
            var initialOffset = origin switch
            {
                SeekOrigin.Begin => 0,
                SeekOrigin.Current => _currentPosition,
                SeekOrigin.End => _currentLength,
                _ => throw new NotSupportedException(),
            };

            var finalOffset = initialOffset + offset;

            var newPosition = finalOffset;

            if (newPosition > _currentLength)
                throw new IOException("Could not seek past the end of the stream.");
            if (newPosition < 0)
                throw new IOException("Could not seek before the end of the stream.");

            _currentPosition = newPosition;
            return _currentPosition;
        }

        private bool TryGetPage(int index, out Byte[]? page)
        {
            if (index < _pages.Count)
            {
                page = GetPage(index);
                return true;
            }
            else
            {
                page = null;
                return false;
            }
        }
    }
}
