// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Collections.Concurrent;

namespace Yextly.Common
{
    /// <summary>
    /// Defines methods to manipulate generic collections.
    /// </summary>
    /// <typeparam name="TElement">The type of the elements in the collection.</typeparam>
    /// <remarks>Each element is guaranteed to be disposed when the collection is disposed. Items removed before the container disposal <b>are not disposed</b> by design since the ownership is assumed to be transferred.</remarks>
    public interface IDisposableProducerConsumerCollection<TElement> : IProducerConsumerCollection<TElement>, IReadOnlyCollection<TElement>, IDisposable where TElement : IDisposable
    {
        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        new int Count { get; }
    }
}
