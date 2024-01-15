// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Yextly.Common;

namespace Yextly.Testing.Mocks.Http
{
    internal sealed class LeakingContainer : IDisposableProducerConsumerCollection<IDisposable>
    {
        public int Count => 0;

        public bool IsSynchronized => false;

        public object SyncRoot { get; } = new();

        public void CopyTo(IDisposable[] array, int index)
        {
        }

        public void CopyTo(Array array, int index)
        {
        }

        public void Dispose()
        {
        }

        public IEnumerator<IDisposable> GetEnumerator()
        {
            return Enumerable.Empty<IDisposable>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Enumerable.Empty<IDisposable>().GetEnumerator();
        }

        public IDisposable[] ToArray()
        {
            return [];
        }

        public bool TryAdd(IDisposable item)
        {
            return true;
        }

        public bool TryTake([MaybeNullWhen(false)] out IDisposable item)
        {
            item = default;
            return false;
        }
    }
}
