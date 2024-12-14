// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace System.Collections.Generic
{
    /// <summary>
    /// Contains <see cref="IEnumerable{T}"/> extensions.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Repeats the elements of <paramref name="source"/> until <paramref name="predicate"/> becomes true. Then, it starts over.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}" /> to return an element from.</param>
        /// <param name="predicate">A function to test when the sequence must be restarted.</param>
        /// <returns>An enuemrable of the same cardinality of <paramref name="source"/></returns>
        public static IEnumerable<TSource> RepeatAfter<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(predicate);

            var list = source.ToList();

            var initialCount = list.Count;

            if (initialCount == 0)
                return list;

            var (_, count) = list
                .Select((x, i) => (Value: x, Count: i + 1))
                .FirstOrDefault(x => predicate(x.Value));

            if (count <= 1)
                return list;

            return
                list.Take(count - 1)
                .RepeatIndefinitely()
                .Take(initialCount);
        }

        /// <summary>
        /// Repeats <paramref name="source"/> indefinitely.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}" /> to return an element from.</param>
        /// <returns></returns>
        [Diagnostics.CodeAnalysis.SuppressMessage("Blocker Bug", "S2190:Loops and recursions should not be infinite", Justification = "This is the purpose of the method.")]
        public static IEnumerable<TSource> RepeatIndefinitely<TSource>(this IEnumerable<TSource> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var captureSource = source is not List<TSource>;

            var capture = (captureSource) ? source.ToList() : source;

            return RepeatIndefinitelyInternal(capture);

            static IEnumerable<TSource> RepeatIndefinitelyInternal(IEnumerable<TSource> source)
            {
                while (true)
                {
                    foreach (var item in source)
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
