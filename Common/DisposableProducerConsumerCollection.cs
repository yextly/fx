// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Yextly.Common
{
    /// <summary>
    /// Represents a container which disposes the contained items at the end of its lifeycle.
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    /// <remarks>We try to detect use-after-free on interface members only. All the other members are not "protected".</remarks>
    internal sealed class DisposableProducerConsumerCollection<TElement> : IDisposableProducerConsumerCollection<TElement>
        where TElement : IDisposable
    {
        private readonly IProducerConsumerCollection<TElement> _container;
        private readonly bool _disposeWithSnapshot;
        private volatile bool _disposed;

        public DisposableProducerConsumerCollection(IProducerConsumerCollection<TElement> container, bool disposeWithSnapshot)
        {
            ArgumentNullException.ThrowIfNull(container);

            _container = container;
            _disposeWithSnapshot = disposeWithSnapshot;
        }

        public int Count
        {
            get
            {
                ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
                return ((ICollection)_container).Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
                return _container.IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
                return _container.SyncRoot;
            }
        }

        public void CopyTo(TElement[] array, int index)
        {
            ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
            _container.CopyTo(array, index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
            _container.CopyTo(array, index);
        }

        public void Dispose()
        {
            Thread.MemoryBarrier();
            if (_disposed)
                return;

            _disposed = true;
            Thread.MemoryBarrier();

            CollectionUtilities.DisposeInternal(_container, _disposeWithSnapshot, Clean);
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
            return _container.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
            return ((IEnumerable)_container).GetEnumerator();
        }

        public TElement[] ToArray()
        {
            ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
            return [.. _container];
        }

        public bool TryAdd(TElement item)
        {
            ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
            return _container.TryAdd(item);
        }

        public bool TryTake([MaybeNullWhen(false)] out TElement item)
        {
            ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
            return _container.TryTake(out item);
        }

        private void Clean()
        {
            while (_container.TryTake(out _))
            {
            }
        }
    }
}
