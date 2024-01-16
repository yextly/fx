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
    public interface IDisposableCollection<TElement> : ICollection<TElement>, IDisposable where TElement : IDisposable;
}
