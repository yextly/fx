// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Common
{
    /// <summary>
    /// Represents a stream that does not propagate disposal.
    /// </summary>
    public sealed class UnclosableStream : Stream
    {
        private readonly NonOwned<Stream> _inner;

        /// <summary>
        /// Constructs a new <see cref="UnclosableStream"/>
        /// </summary>
        /// <param name="inner">The inner stream.</param>
        public UnclosableStream(Stream inner)
        {
            ArgumentNullException.ThrowIfNull(inner);

            _inner = inner.AsNonOwned();
        }

        /// <inheritdoc/>
        public override bool CanRead => _inner.Value.CanRead;

        /// <inheritdoc/>
        public override bool CanSeek => _inner.Value.CanSeek;

        /// <inheritdoc/>
        public override bool CanWrite => _inner.Value.CanWrite;

        /// <inheritdoc/>
        public override long Length => _inner.Value.Length;

        /// <inheritdoc/>
        public override long Position { get => _inner.Value.Position; set => _inner.Value.Position = value; }

        /// <inheritdoc/>
        public override void Flush()
        {
            _inner.Value.Flush();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _inner.Value.Read(buffer, offset, count);
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _inner.Value.Seek(offset, origin);
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            _inner.Value.SetLength(value);
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _inner.Value.Write(buffer, offset, count);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            using (_inner)
            {
            }
            base.Dispose(disposing);
        }
    }
}
