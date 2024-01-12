// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Collections.Concurrent;
using Yextly.Common;

namespace System.Collections.Generic
{
    /// <summary>
    /// Contains extensions that turn collections into disposable container.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Creates a wrapper container which disposes the contained items at the end of its lifeycle.
        /// </summary>
        /// <remarks>The elements of the collection are disposed only when the container is disposed.</remarks>
        /// <typeparam name="TElement">The type of the elements contained iin the container</typeparam>
        /// <param name="source">The source container.</param>
        /// <param name="safe"><see langword="true"/> To shapshot the collection before disposing, <see langword="false"/> to enumerate it in place.</param>
        /// <returns></returns>
        public static IDisposableCollection<TElement> AsDisposableCollection<TElement>(this ICollection<TElement> source, bool safe = false)
            where TElement : IDisposable
        {
            ArgumentNullException.ThrowIfNull(source);

            return new DisposableCollection<TElement>(source, safe);
        }

        /// <summary>
        /// Creates a wrapper container which disposes the contained items at the end of its lifeycle.
        /// </summary>
        /// <remarks>The elements of the collection are disposed only when the container is disposed.</remarks>
        /// <typeparam name="TElement">The type of the elements contained iin the container</typeparam>
        /// <param name="source">The source container.</param>
        /// <param name="safe"><see langword="true"/> To shapshot the collection before disposing, <see langword="false"/> to enumerate it in place.</param>
        /// <returns></returns>
        public static IDisposableProducerConsumerCollection<TElement> AsDisposableCollection<TElement>(this IProducerConsumerCollection<TElement> source, bool safe = true)
            where TElement : IDisposable
        {
            ArgumentNullException.ThrowIfNull(source);

            return new DisposableProducerConsumerCollection<TElement>(source, safe);
        }

        /// <summary>
        /// Creates a wrapper container which disposes the contained items at the end of its lifeycle.
        /// </summary>
        /// <remarks>The elements of the collection are disposed only when the container is disposed.</remarks>
        /// <typeparam name="TElement">The type of the elements contained iin the container</typeparam>
        /// <param name="source">The source container.</param>
        /// <param name="safe"><see langword="true"/> To shapshot the collection before disposing, <see langword="false"/> to enumerate it in place.</param>
        /// <returns></returns>
        public static IDisposableList<TElement> AsDisposableList<TElement>(this IList<TElement> source, bool safe = false)
            where TElement : IDisposable
        {
            ArgumentNullException.ThrowIfNull(source);

            return new DisposableList<TElement>(source, safe);
        }
    }
}
