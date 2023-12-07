// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Collections.Immutable;
using Xunit;

namespace Yextly.Xunit
{
    /// <summary>
    /// Provides a convenient type-safe way to combine multiple theories into a single one.
    /// </summary>
    public abstract class CombinedTheoryData : TheoryData
    {
        /// <summary>
        /// Creates a <see cref="CombinedTheoryData"/> instance.
        /// </summary>
        /// <param name="source">The list of types to handle.</param>
        protected CombinedTheoryData(params Type[] source)
        {
            _ = source.All(x => typeof(TheoryData).IsAssignableFrom(x));
            _ = source.All(x => x.GetConstructor(Type.EmptyTypes) != null);

            var s = source.Select(x => x.GetConstructor(Type.EmptyTypes)!.Invoke(null)).Cast<TheoryData>().ToList();

            var data = s.ConvertAll(x => x.ToList());

            var offsets = data.Select(x => x.FirstOrDefault()?.Length).Select(x => x ?? 0).ToList();

            var totalSize = offsets.Sum();

            var counter = new Counter(data.Select(x => x.Count).ToArray());

            while (counter.Next())
            {
                var a = new object[totalSize];
                int offset = 0;
                for (int i = 0; i < data.Count; i++)
                {
                    data[i][counter.Current[i]].CopyTo(a, offset);
                    offset += offsets[i];
                }

                AddRow(a);
            }
        }

        internal sealed class Counter
        {
            public readonly int[] Current;
            public readonly ImmutableArray<int> Sizes;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "Broken in net6")]
            public Counter(int[] count)
            {
                Sizes = count.ToImmutableArray();
                Current = new int[Sizes.Length];
                Current[0] = -1;
            }

            public bool Next()
            {
                return Inc();
            }

            private bool Inc()
            {
                for (int i = 0; i < Current.Length; i++)
                {
                    var good = ++Current[i] < Sizes[i];

                    if (good)
                        return true;
                    Current[i] = 0;
                }

                return false;
            }
        }
    }

    /// <inheritdoc/>
    public class CombinedTheoryData<T> : CombinedTheoryData
    {
        /// <summary>
        /// Creates a <see cref="CombinedTheoryData{T}"/> instance.
        /// </summary>
        public CombinedTheoryData() : base(typeof(T))
        {
        }
    }

    /// <inheritdoc/>
    public class CombinedTheoryData<T1, T2> : CombinedTheoryData
    {
        /// <summary>
        /// Creates a <see cref="CombinedTheoryData{T1,T2}"/> instance.
        /// </summary>
        public CombinedTheoryData() : base(typeof(T1), typeof(T2))
        {
        }
    }

    /// <inheritdoc/>
    public class CombinedTheoryData<T1, T2, T3> : CombinedTheoryData
    {
        /// <summary>
        /// Creates a <see cref="CombinedTheoryData{T1,T2,T3}"/> instance.
        /// </summary>
        public CombinedTheoryData() : base(typeof(T1), typeof(T2), typeof(T3))
        {
        }
    }

    /// <inheritdoc/>
    public class CombinedTheoryData<T1, T2, T3, T4> : CombinedTheoryData
    {
        /// <summary>
        /// Creates a <see cref="CombinedTheoryData{T1,T2,T3,T4}"/> instance.
        /// </summary>
        public CombinedTheoryData() : base(typeof(T1), typeof(T2), typeof(T3), typeof(T4))
        {
        }
    }

    /// <inheritdoc/>
    public class CombinedTheoryData<T1, T2, T3, T4, T5> : CombinedTheoryData
    {
        /// <summary>
        /// Creates a <see cref="CombinedTheoryData{T1,T2,T3,T4,T5}"/> instance.
        /// </summary>
        public CombinedTheoryData() : base(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5))
        {
        }
    }

    /// <inheritdoc/>
    public class CombinedTheoryData<T1, T2, T3, T4, T5, T6> : CombinedTheoryData
    {
        /// <summary>
        /// Creates a <see cref="CombinedTheoryData{T1,T2,T3,T4,T5,T6}"/> instance.
        /// </summary>
        public CombinedTheoryData() : base(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6))
        {
        }
    }

    /// <inheritdoc/>
    public class CombinedTheoryData<T1, T2, T3, T4, T5, T6, T7> : CombinedTheoryData
    {
        /// <summary>
        /// Creates a <see cref="CombinedTheoryData{T1,T2,T3,T4,T5,T6,T7}"/> instance.
        /// </summary>
        public CombinedTheoryData() : base(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7))
        {
        }
    }

    /// <inheritdoc/>
    public class CombinedTheoryData<T1, T2, T3, T4, T5, T6, T7, T8> : CombinedTheoryData
    {
        /// <summary>
        /// Creates a <see cref="CombinedTheoryData{T1,T2,T3,T4,T5,T6,T7,T8}"/> instance.
        /// </summary>
        public CombinedTheoryData() : base(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8))
        {
        }
    }

    /// <inheritdoc/>
    public class CombinedTheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9> : CombinedTheoryData
    {
        /// <summary>
        /// Creates a <see cref="CombinedTheoryData{T1,T2,T3,T4,T5,T6,T7,T8,T9}"/> instance.
        /// </summary>
        public CombinedTheoryData() : base(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9))
        {
        }
    }
}
