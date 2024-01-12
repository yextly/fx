// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Collections;

namespace Yextly.Common
{
    /// <summary>
    /// Represents a container which disposes the contained items at the end of its lifeycle.
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    /// <remarks>We try to detect use-after-free on interface members only. All the other members are not "protected".</remarks>
    internal sealed class DisposableList<TElement> : IDisposableList<TElement>
       where TElement : IDisposable
    {
        private readonly IList<TElement> _container;
        private readonly bool _disposeWithSnapshot;
        private volatile bool _disposed;

        public DisposableList(IList<TElement> container, bool disposeWithSnapshot)
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
                return _container.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
                return _container.IsReadOnly;
            }
        }

        public TElement this[int index]
        {
            get
            {
                ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
                return ((IList<TElement>)_container)[index];
            }
            set
            {
                ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
                ((IList<TElement>)_container)[index] = value;
            }
        }

        public void Add(TElement item)
        {
            ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
            _container.Add(item);
        }

        public void Clear()
        {
            ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
            _container.Clear();
        }

        public bool Contains(TElement item)
        {
            ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
            return _container.Contains(item);
        }

        public void CopyTo(TElement[] array, int arrayIndex)
        {
            ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
            _container.CopyTo(array, arrayIndex);
        }

        public void Dispose()
        {
            Thread.MemoryBarrier();
            if (_disposed)
                return;

            _disposed = true;
            Thread.MemoryBarrier();

            CollectionUtilities.DisposeInternal(_container, _disposeWithSnapshot, () => _container.Clear());
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

        public int IndexOf(TElement item)
        {
            ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
            return _container.IndexOf(item);
        }

        public void Insert(int index, TElement item)
        {
            ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
            _container.Insert(index, item);
        }

        public bool Remove(TElement item)
        {
            ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
            return _container.Remove(item);
        }

        public void RemoveAt(int index)
        {
            ObjectDisposedExceptionThrowHelper.ThrowIf(_disposed, this);
            _container.RemoveAt(index);
        }
    }
}
